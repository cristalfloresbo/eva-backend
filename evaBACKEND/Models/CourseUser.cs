using evaBACKEND.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace evaBACKEND.Models
{
    public class CourseUser
    {
        public long CourseId { get; set; }

        [NotMapped]
        public Course Course { get; set; }

        public string Id { get; set; }

        [NotMapped]
        public AppUser User { get; set; }
    }
}
