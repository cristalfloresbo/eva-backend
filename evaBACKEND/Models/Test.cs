﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace evaBACKEND.Models
{
	public class Test
	{
		public long TestId { get; set; }

		[Required]
		public string Title { get; set; }

		public DateTime Date { get; set; }

		public Course Course { get; set; }

		[NotMapped]
		public ICollection<Question> Questions { get; set; }
	}
}