using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Lexicon_LMS.Models
{
    public class ApplicationUser : IdentityUser
    {
       
        public string Name { get; set; }

        public ICollection<ApplicationUserCourse> Courses { get; set; }

        internal Task GeneratePasswordResetTokenAsync(ApplicationUser user)
        {
            throw new NotImplementedException();
        }
    }
}
