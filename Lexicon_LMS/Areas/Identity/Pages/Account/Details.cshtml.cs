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
    public class DetailsModel : PageModel
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<RegisterModel> _logger;
        private readonly IEmailSender _emailSender;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ApplicationDbContext _context;

        public DetailsModel(
            UserManager<ApplicationUser> userManager,
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
        public int? CourseId { get; set; }

        public class InputModel
        {
            [Display(Name = "Namn")]
            public string Name { get; set; }

            [Display(Name = "Epost")]
            public string Email { get; set; }

            [Display(Name = "Kurs")]
            public string CourseName { get; set; }

            [Required(ErrorMessage = "{0} måste anges")]
            [Display(Name = "Användartyp")]
            public string Role { get; set; }
        }

        public async Task<IActionResult> OnGetAsync(string userEmail = "", string returnTo = null, int? courseId = 0)
        {
            ReturnUrl = returnTo;
            CourseId = courseId;

            if (userEmail != "")
            {
                // ReturnUrl = "/Identity/Account/Details?userEmail=" + userEmail;
                ReturnUrl = returnTo;

                var user = await _userManager.FindByNameAsync(userEmail);  // .FindByEmailAsync(userEmail);

                if (user != null)
                {
                    Input = new InputModel();
                    Input.Email = user.Email;
                    Input.Name = user.Name;
                    Input.Role = "";

                    var roleType = await _userManager.GetRolesAsync(user);
                    if (roleType != null)
                    {
                        Input.Role = roleType.ElementAt(0);
                    }

                    var Course = _context.UserCourse.FirstOrDefault(c => c.ApplicationUserId == user.Id);
                    if (Course != null)
                    {
                        var CourseId = Course.CourseId;
                        Input.CourseName = _context.Course.FirstOrDefault(v => v.Id == CourseId).Name;
                    }
                    else
                        Input.CourseName = "";
                }
            }
            else if (returnTo != "")
            {
                ReturnUrl = "/Identity/Account/Details?userEmail=" + userEmail;
            }

            return Page();
        }
        
    }
}
