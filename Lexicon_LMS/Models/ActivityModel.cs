using System;
using System.ComponentModel.DataAnnotations;

namespace Lexicon_LMS.Models
{
    public class ActivityModel
    {
        [Key]
        [Required]
        public int Id { get; set; }

		// foreign key
		public int ActivityTypeId { get; set; }
		// navigation reference
		[Display(Name = "Aktivitet Typ")]
		public ActivityType ActivityType { get; set; }

        // Navigational reference 
        public int? ModuleId { get; set; }

        [Required]
        [Display(Name = "Namn")]
        public string Name { get; set; }

        [Display(Name = "Startdatum")]
        public DateTime StartDate { get; set; }

        [Display(Name = "Slutdatum")]
        public DateTime StopDate { get; set; }

        [Required]
        [Display(Name = "Beskrivning")]
        public string Description { get; set; }
    }
}
