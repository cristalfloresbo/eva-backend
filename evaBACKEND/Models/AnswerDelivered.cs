using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace evaBACKEND.Models
{
	public class AnswerDelivered
	{
		[Key, Column(Order = 2)]
		public long QuestionId { get; set; }

		[Key, Column(Order = 1)]
		public long AnswerId { get; set; }

		[JsonIgnore]
		public Question Question { get; set; }

		[JsonIgnore]
		public Answer Answer { get; set; }

	}
}
