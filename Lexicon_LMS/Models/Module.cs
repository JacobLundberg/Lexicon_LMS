using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace Lexicon_LMS.Models
{
	public class Module
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
		[DataType(DataType.Date)]
		public DateTime StartTime { get; set; }

		[Display(Name = "Slutdatum")]
		[DataType(DataType.Date)]
		public DateTime EndTime { get; set; }

		[Display(Name = "Aktivitet")]
		public ICollection<ActivityModel> ModuleActivities { get; set; }

	}
}
