using System;using Lexicon_LMS.Models;
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

namespace Lexicon_LMS.Areas.Identity.Pages
{
    
    [Authorize(Roles = "Teacher")]
    public class RegisterModel : PageModel
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<RegisterModel> _logger;
        private readonly IEmailSender _emailSender;

        public RegisterModel(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            ILogger<RegisterModel> logger,
            IEmailSender emailSender)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _emailSender = emailSender;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public string ReturnUrl { get; set; }

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
            // [DataType(DataType.Password)]
            [Display(Name = "Lösenord")]
            public string Password { get; set; }

            // [DataType(DataType.Password)]
            [Display(Name = "Upprepa lösenord")]
            [Compare("Password", ErrorMessage = "Lösenorden är inte lika.")]
            public string ConfirmPassword { get; set; }

            [Required(ErrorMessage = "{0} måste anges")]
            [Display(Name = "Användartyp")]
            public string Role { get; set; }

            // public IEnumerable<SelectListItem> Roles {get; set;}
        }

        public void OnGet(string returnUrl = null)
        {
            ReturnUrl = returnUrl;
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl = returnUrl ?? Url.Content("~/");
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser { Name = Input.Name, UserName = Input.Email, Email = Input.Email };
                var result = await _userManager.CreateAsync(user, Input.Password);
                if (result.Succeeded)
                {
                    _logger.LogInformation("User created a new account with password.");

                    //--- Koppla User till Role ---------------------
                    var svar = await _userManager.AddToRoleAsync(user, Input.Role);  // roleManager???
                    if (!svar.Succeeded)
                    {
                        throw new Exception(string.Join("\n", svar.Errors));
                    }

                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    var callbackUrl = Url.Page(
                        "/Account/ConfirmEmail",
                        pageHandler: null,
                        values: new { userId = user.Id, code = code },
                        protocol: Request.Scheme);

                    //await _emailSender.SendEmailAsync(Input.Email, "Confirm your email",
                    //    $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

                    // await _signInManager.SignInAsync(user, isPersistent: false);
                    return LocalRedirect(returnUrl);
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            // If we got this far, something failed, redisplay form
            return Page();
        }
    }
}
