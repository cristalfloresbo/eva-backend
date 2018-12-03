using evaBACKEND.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace evaBACKEND.Models
{
    public class Course
    {
        public long CourseId { get; set; }

        public String Name { get; set; }

        public String Description { get; set; }

        public AppUser Teacher { get; set; }

        [NotMapped]
        public List<AppUser> Students { get; set; }

        public DateTime StartDate { get; set; }

        public ICollection<Task> Tasks { get; set; }
	}
}
