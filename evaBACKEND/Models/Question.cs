using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace evaBACKEND.Models
{
	public class Question
	{
		public long QuestionId { get; set; }

		[Required]
		public string Value { get; set; }

		[NotMapped]
		public ICollection<Answer> Answers { get; set; }

		public Test Test { get; set; }
	}
}
