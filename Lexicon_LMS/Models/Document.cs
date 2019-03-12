using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Lexicon_LMS.Models
{
	public class Document
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

		[Required]
		[Display(Name = "Timestamp")]
		public DateTime Timestamp { get; set; }

		[Required]
		[Display(Name = "Användare")]
		[ForeignKey("ApplicationUserId")]
		public int ApplicationUserId { get; set; }

		[Display(Name = "Kurs")]
		[ForeignKey("CourseId")]
		public int? CourseId { get; set; }

		[Display(Name = "Modul")]
		[ForeignKey("ModuleId")]
		public int? ModuleId { get; set; }

		[Display(Name = "Aktivitet")]
		[ForeignKey("ActivityId")]
		public int? ActivityId { get; set; }
	}
}