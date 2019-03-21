using System;
using Lexicon_LMS.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using Lexicon_LMS.Data;

namespace Lexicon_LMS.Areas.Identity.Pages
{
    [Authorize(Roles = "Teacher")]
    public class RegisterModel : PageModel
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<RegisterModel> _logger;
        private readonly IEmailSender _emailSender;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ApplicationDbContext _context;

        public RegisterModel(UserManager<ApplicationUser> userManager,
                             SignInManager<ApplicationUser> signInManager, 
                             ILogger<RegisterModel> logger, 
                             IEmailSender emailSender, 
                             RoleManager<IdentityRole> roleManager,
                             ApplicationDbContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _emailSender = emailSender;
            _roleManager = roleManager;
            _context = context;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public string ReturnUrl { get; set; }
        public object ViewBag { get; private set; }

        public string CourseName { get; set; }
        //public string UserRole { get; set; }

        public class InputModel
        {
            [Required(ErrorMessage = "{0} måste anges")]
            [StringLength(30, ErrorMessage = "{0} måste vara minst {2} och högst {1} tecken långt.", MinimumLength = 3)]
            [Display(Name = "Namn")]
            public string Name { get; set; }

            [Required(ErrorMessage = "{0} måste anges")]
            [EmailAddress]
            [Display(Name = "Epost")]
            public string Email { get; set; }

            [Required(ErrorMessage = "{0} måste anges")]
            [StringLength(100, ErrorMessage = "{0}et måste var minst {2} och högst {1} tecken långt.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Lösenord")]
            public string Password { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "Upprepa lösenord")]
            [Compare("Password", ErrorMessage = "Lösenorden är inte lika.")]
            public string ConfirmPassword { get; set; }

            [Required(ErrorMessage = "{0} måste anges")]
            [StringLength(20, MinimumLength = 1)]
            [Display(Name = "Användartyp")]
            public string Role { get; set; }
            public IEnumerable<SelectListItem> Roles { get; set; }

            [Display(Name = "Koppla till kurs")]
            public int CourseId { get; set; }
            public IEnumerable<SelectListItem> Courses { get; set; }
        }

        public IActionResult OnGet(string returnUrl = null, int courseId = 0)
        {
            ReturnUrl = returnUrl;

            if (Input == null)
            {
                Input = new InputModel();
                Input.CourseId = courseId;

                if (courseId > 0)
                {
                    Input.Courses = new SelectList(_context.Course.Where(c => c.Id == courseId).ToList(), "Id", "Name", courseId);
                    CourseName = _context.Course.Where(c => c.Id == courseId).SingleOrDefault().Name.ToString();
                    Input.Roles = new SelectList(_roleManager.Roles.Where(r => r.Name == "Student").ToList(), "Name", "Name", "Student");  // , "Student"
                }
                else
                {
                    Input.Roles = new SelectList(_roleManager.Roles, "Name", "Name");
                    Input.Courses = new SelectList(_context.Course.ToList(), "Id", "Name", Input.CourseId);
                }
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser { Name = Input.Name, UserName = Input.Email, Email = Input.Email };

                if (user != null)
                {
                    // Skapa användaren
                    var result = await _userManager.CreateAsync(user, Input.Password);

                    if (result.Succeeded)
                    {
                        _logger.LogInformation("User created a new account with password.");

                        // Koppla användaren till en roll
                        var svar = await _userManager.AddToRoleAsync(user, Input.Role);
                        if (!svar.Succeeded)
                        {
                            throw new Exception(string.Join("\n", svar.Errors));
                        }

                        #region SendEmail
                        //var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                        //var callbackUrl = Url.Page(
                        //    "/Account/ConfirmEmail",
                        //    pageHandler: null,
                        //    values: new { userId = user.Id, code = code },
                        //    protocol: Request.Scheme);

                        //await _emailSender.SendEmailAsync(Input.Email, "Confirm your email",
                        //    $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

                        // await _signInManager.SignInAsync(user, isPersistent: false);
                        #endregion

                        if (Input.CourseId > 0)  // Koppla användare till kurs
                        {
                            // Koppla användare till en kurs
                            var newCourse = new ApplicationUserCourse
                            {
                                ApplicationUserId = user.Id,
                                CourseId = Input.CourseId
                            };

                            _context.Add(newCourse);
                            _context.SaveChanges();
                        }

                        if (returnUrl == "" || returnUrl == null) {
                            string roll = "";
                            if (Input.Role == "Teacher")
                                roll = "lärare";
                            else
                                roll = "elev";

                            TempData["newUser"] = "Skapade ny " + roll;
                            TempData["newUserData"] = Input.Name + " (" + Input.Email + ")";

                            returnUrl = "/Identity/Account/Details?userEmail=" + Input.Email;
                        }
                        else
                        {
                            TempData["newUserData"] = "Skapade ny elev på kursen: " + Input.Name + " (" + Input.Email + ")";

                            returnUrl = returnUrl + "/" + Input.CourseId;  //  courseId=2&returnUrl=/Courses/Details
                        }

                        return LocalRedirect(returnUrl);
                    }
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }
            }

            // Skapa data vid fel
            if (Input.CourseId > 0)
            {
                // Bara student
                Input.Roles = new SelectList(_roleManager.Roles.Where(r => r.Name == "Student").ToList(), "Name", "Name", "Student");  // , "Student"
                // Bara rätt kurs i listan
                Input.Courses = new SelectList(_context.Course.Where(c => c.Id == Input.CourseId).ToList(), "Id", "Name", Input.CourseId);
                // Kursens namn
                CourseName = _context.Course.Where(c => c.Id == Input.CourseId).SingleOrDefault().Name.ToString();
            }
            else
            {
                // Lista roller
                Input.Roles = new SelectList(_roleManager.Roles, "Name", "Name");
                // Lista kurser
                Input.Courses = new SelectList(_context.Course.ToList(), "Id", "Name", Input.CourseId);
            }


            // If we got this far, something failed, redisplay form
            return Page();
        }
    }
}
