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
    public class EditUserModel : PageModel
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<RegisterModel> _logger;
        private readonly IEmailSender _emailSender;
        private readonly RoleManager<IdentityRole> _roleManager;

        public EditUserModel(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            ILogger<RegisterModel> logger,
            IEmailSender emailSender,
            RoleManager<IdentityRole> roleManager)
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

            //=== PASSWORD AVSTÄNGD ===
            //// [Required(ErrorMessage = "{0} måste anges")]
            //[StringLength(100, ErrorMessage = "{0}et måste var minst {2} och högst {1} tecken långt.", MinimumLength = 6)]
            //[DataType(DataType.Password)]
            //[Display(Name = "Lösenord")]
            //public string Password { get; set; }

            //[DataType(DataType.Password)]
            //[Display(Name = "Upprepa lösenord")]
            //[Compare("Password", ErrorMessage = "Lösenorden är inte lika.")]
            //public string ConfirmPassword { get; set; }

            [Required(ErrorMessage = "{0} måste anges")]
            [Display(Name = "Användartyp")]
            public string Role { get; set; }

            [Display(Name= "Select")]
            public IEnumerable<SelectListItem> Roles { get; set; }

            [HiddenInput]
            public string OrgEmail { get; set; }
        }

        public async Task<IActionResult> OnGetAsync(string returnUrl = null, string userEmail = "")
        {
            ReturnUrl = returnUrl;

            if (userEmail != "")
            {
                var user = await _userManager.FindByEmailAsync(userEmail);

                Input = new InputModel();
                if (user != null) {
                    Input.Name = user.Name;
                    Input.Email = user.Email;
                    Input.OrgEmail = userEmail;

                    var role = "";

                    var roleType = await _userManager.GetRolesAsync(user);
                    if (roleType != null)
                    {
                        role = roleType.ElementAt(0);
                    }
                    Input.Roles = new SelectList(_roleManager.Roles, "Name", "Name", role);
                }

                return Page();
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            // returnUrl = returnUrl ?? Url.Content("~/");
            if (returnUrl == null)
                returnUrl = "";

            if (!ModelState.IsValid)
            {
                return Page();
            }
            
            ApplicationUser user = await _userManager.FindByEmailAsync(Input.OrgEmail);
            if (user == null)
            {
                // return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
                return NotFound($"Unable to load user with ID '{_userManager.FindByEmailAsync(Input.OrgEmail)}'.");
            }

            var memEmail = user.Email;
            var tmpRoles = await _userManager.GetRolesAsync(user);
            var orgRole = tmpRoles.First();

            Boolean changed = false;
            Boolean changedRole = false;

            if (Input.Email != user.Email) {
                memEmail = Input.Email;  // lagra email för retur
                user.UserName = Input.Email;
                user.Email = Input.Email;
                changed = true;
            }

            if (Input.Name != user.Name)
            {
                user.Name = Input.Name;
                changed = true;
            }

            if (Input.Role != orgRole)
            {
                changedRole = true;
            }

            //=== PASSWORD AVSTÄNGD ===
            // Password om lika + checked...
            // user.PasswordHash

            IdentityResult updateUser = null;
            IdentityResult updateRole = null;

            if (changed || changedRole)
            {
                if (changed)
                { 
                    // Ändra användardata (ej roll)
                    updateUser = await _userManager.UpdateAsync(user);
                    if (!updateUser.Succeeded)
                    {
                        var userId = await _userManager.FindByIdAsync(user.Id.ToString()); //  .GetUserIdAsync(user);
                        throw new InvalidOperationException($"Unexpected error occurred setting email for user with ID '{userId}'.");
                    }
                }

                if (changedRole)
                {
                    //--- Koppla User från Role ---------------------
                    updateRole = await _userManager.RemoveFromRoleAsync(user, orgRole);
                    if (!updateRole.Succeeded)
                    {
                        throw new Exception(string.Join("\n", updateRole.Errors));
                    }

                    //--- Koppla User till Role ---------------------
                    updateRole = await _userManager.AddToRoleAsync(user, Input.Role);
                    if (!updateRole.Succeeded)
                    {
                        throw new Exception(string.Join("\n", updateRole.Errors));
                    }
                }

                TempData["newUser"] = "Ändrade " + Input.Name;  //  "Skapade användaren '" + Input.Name + "' (" + Input.Role + ")";
                TempData["newUserData"] = "";  // Input.Name + " (" + Input.Email + ")";

                if (returnUrl == "")
                {
                    // if (returnUrl == "details")
                        return LocalRedirect("/Identity/Account/Details?userEmail=" + user.UserName);  // /Identity/Account/Details?userEmail=s@s.com
                }
            }

            user = await _userManager.FindByIdAsync(user.Id.ToString());

            //if (Input.Email != user.Email)
            //    user = await _userManager.FindByEmailAsync(Input.OrgEmail);
            //else
            //    user = await _userManager.FindByEmailAsync(Input.OrgEmail);

            Input.Roles = new SelectList(_roleManager.Roles, "Name", "Name");
            return Page();
        }
    }
}
