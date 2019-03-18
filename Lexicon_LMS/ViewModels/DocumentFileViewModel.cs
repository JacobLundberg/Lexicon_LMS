using System;
using System.ComponentModel.DataAnnotations;

namespace Lexicon_LMS.ViewModels
{
    public class DocumentFileViewModel
    {
        [Required]
        [Display(Name = "Namn")]
        public string Name { get; set; }

        [Required]
        [Display(Name = "Beskrivning")]
        public string Description { get; set; }

        [Display(Name = "Timestamp")]
        public DateTime Timestamp { get; set; }

        [Display(Name = "Användare")]
        public string ApplicationUserId { get; set; }

        [Display(Name = "Kurs")]
        public int? CourseId { get; set; }

        [Display(Name = "Modul")]
        public int? ModuleId { get; set; }

        [Display(Name = "Aktivitet")]
        public int? ActivityId { get; set; }

        public LMSFormFile LMSFormFile { get; set; }
    }
}
