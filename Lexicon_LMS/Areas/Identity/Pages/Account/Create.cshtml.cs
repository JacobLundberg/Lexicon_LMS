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

        public RegisterModel(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, ILogger<RegisterModel> logger, IEmailSender emailSender, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _emailSender = emailSender;
            _roleManager = roleManager;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public string ReturnUrl { get; set; }
        public object ViewBag { get; private set; }

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
            [Display(Name = "Användartyp")]
            public string Role { get; set; }

            [Display(Name= "Select")]
            public IEnumerable<SelectListItem> Roles { get; set; }
        }

        public IActionResult OnGet(string returnUrl = null)
        {
            if (Input == null)
            {
                Input = new InputModel();
                Input.Roles = new SelectList(_roleManager.Roles, "Name", "Name");
            }

            ReturnUrl = returnUrl;

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            // returnUrl = returnUrl ?? Url.Content("~/");
            // returnUrl = returnUrl ?? Url.Content("/Areas/Identity/Pages/Account/Details");
            // returnUrl = returnUrl ?  ? Url.Content("/Index");
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser { Name = Input.Name, UserName = Input.Email, Email = Input.Email };
                var result = await _userManager.CreateAsync(user, Input.Password);

                if (result.Succeeded)
                {
                    _logger.LogInformation("User created a new account with password.");

                    //--- Koppla User till Role ---------------------
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

                    string roll = "";
                    if (Input.Role == "Teacher")
                        roll = "lärare";
                    else
                        roll = "elev";

                    TempData["newUser"] = "Skapade ny " + roll;  //  "Skapade användaren '" + Input.Name + "' (" + Input.Role + ")";

                    if (returnUrl == "" || returnUrl == null)
                        returnUrl = "/Identity/Account/Details?userEmail=" + Input.Email;
                    else
                    {
                        TempData["newUserData"] = Input.Name + " (" + Input.Email + ")";
                        returnUrl = "/Identity/Account";
                    }

                    return LocalRedirect(returnUrl);
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
            //else
            //    returnUrl = returnUrl ?? Url.Content("/Areas/Identity/Pages/Account");  // /Areas/Identity/Pages/Account/Details
            // If we got this far, something failed, redisplay form
            Input.Roles = new SelectList(_roleManager.Roles, "Name", "Name");
            return Page();
        }
    }
}
