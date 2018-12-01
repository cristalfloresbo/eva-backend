using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace evaBACKEND.Models
{
	public class Task
	{
		[Key]
		[Column(Order = 1)]
		public long ID { get; set; }

		[Required]
		public string Title { get; set; }

		[Required]
		public string Comment { get; set; }

		[Required]
		public DateTime DeliveryDate { get; set; }

		[Required]
		public Course Course { get; set; }
	}
}
