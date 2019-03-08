using System.ComponentModel.DataAnnotations;

namespace Lexicon_LMS.Models
{
    public class ActivityType
    {
        [Key]
        [Required]
        public int Id { get; set; }

        [Required]
        [Display(Name = "Namn")]
        public string Name { get; set; }

        [Display(Name = "Beskrivning")]
        public string Description { get; set; }

    }
}
