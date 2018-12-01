using evaBACKEND.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace evaBACKEND.Models
{
	public class Grade
	{
		public long ID { get; set; }

		public AppUser Student { get; set; }

		public Course Course { get; set; }

		public int Value { get; set; }

		public bool IsApproved { get; set; }
	}
}
