using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Lexicon_LMS.Models
{
    public class ActivityModel
    {
        [Key]
        [Required]
        public int Id { get; set; }

        // foreign key
        [Display(Name = "Aktivitetstyp")]
		[ForeignKey("ActivityTypeId")]
		public int? ActivityTypeId { get; set; }
        // navigation reference
        [Display(Name = "Aktivitetstyp")]
        public ActivityType ActivityType { get; set; }

		// Navigational reference 
		[ForeignKey("ModuleId")]
		public int ModuleId { get; set; }

        [Required]
        [Display(Name = "Namn")]
        public string Name { get; set; }

        [Display(Name = "Startdatum")]
        public DateTime StartDate { get; set; }

        [Display(Name = "Slutdatum")]
        public DateTime StopDate { get; set; }

        [Required]
        [Display(Name = "Aktivitetsbeskrivning")]
        public string Description { get; set; }
    }
}
