using Microsoft.AspNetCore.Identity;

namespace Lexicon_LMS.Models
{
    public class ApplicationUser : IdentityUser
    {
//        public ICollection<ApplicationUserGymClass> AttendedClasses { get; set; }

        public string Name { get; set; }
//        public int Age { get; set; }

    }
}
