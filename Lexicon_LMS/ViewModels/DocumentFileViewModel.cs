using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

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

        [Required]
        [Display(Name = "Timestamp")]
        public DateTime Timestamp { get; set; }

        [Required]
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
