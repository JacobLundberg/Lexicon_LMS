using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Lexicon_LMS.Models
{
    public class ApplicationUserCourse
	{
        //[Key]
        //public int Id { get; set; }

        [ForeignKey("CourseId")]
        public int CourseId { get; set; }

        [ForeignKey("ApplicationUserId")]
        public string ApplicationUserId { get; set; }

		public ApplicationUser ApplicationUser { get; set; }
		public Course Course { get; set; }
	}
}
