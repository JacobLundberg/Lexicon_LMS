using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;


namespace Lexicon_LMS.Models
{
    public class Course
    {
        [Key]
        [Required]
        public int Id { get; set; }

        [Required]
        [Display(Name = "Namn")]
        public string Name { get; set; }

        [Required]
        [Display(Name = "Beskrivning")]
        public string Description { get; set; }

        [Display(Name = "Startdatum")]
        public DateTime StartDate { get; set; }

        public ICollection<Module> Modules { get; set; }

		[Display(Name = "Användare")]
		public ICollection<ApplicationUser> ApplicationUsers { get; set; }
	}
}



