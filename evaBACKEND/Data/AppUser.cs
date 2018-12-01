using evaBACKEND.Models;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace evaBACKEND.Data
{
    public class AppUser : IdentityUser
    {
		public string FirstName { get; set; }

		public string LastName { get; set; }

		[NotMapped]
		public List<Grade> Grades {get; set; }
	}
}