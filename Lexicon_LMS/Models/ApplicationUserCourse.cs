namespace Lexicon_LMS.Models
{
    public class ApplicationUserCourse
	{
		public int CourseId { get; set; }
		public string ApplicationUserId { get; set; }

		public ApplicationUser ApplicationUser { get; set; }
		public Course Course { get; set; }
	}
}
