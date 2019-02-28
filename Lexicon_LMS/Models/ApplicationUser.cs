using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;

namespace Lexicon_LMS.Models
{
    public class ApplicationUser : IdentityUser
    {
       
        public string Name { get; set; }

		public ICollection<ApplicationUserCourse> Courses { get; set; }

	}
}
