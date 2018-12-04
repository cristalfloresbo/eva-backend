using evaBACKEND.Data;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace evaBACKEND.Models
{
	public class TestDelivered
	{
		[Key, Column(Order = 2)]
		public string StudentId { get; set; }

		[Key, Column(Order = 1)]
		public long TestId { get; set; }

		public DateTime deliveryDate { get; set; }

		public Grade Grade { get; set; }

		public ICollection<AnswerDelivered> Answers { get; set; }

		[JsonIgnore]
		public AppUser Student { get; set; }

		[JsonIgnore]
		public Test Test { get; set; }

		public TestDelivered()
		{
			deliveryDate = DateTime.Now;
		}
	}
}
