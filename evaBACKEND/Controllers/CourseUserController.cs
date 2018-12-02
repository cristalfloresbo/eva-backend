using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using evaBACKEND.Data;
using evaBACKEND.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace evaBACKEND.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CourseUserController : ControllerBase
    {
        private readonly AppDbContext _context;

        public CourseUserController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public IActionResult AddUserToCourse([FromBody] CourseUser courseUser)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userRole = _context.UserRoles.Where(ur => ur.UserId == courseUser.Id).First();
            var role = _context.Roles.Where(r => r.Id == userRole.RoleId).First();
            if (role.Name != "Admin")
            {
                return BadRequest("User pretend to add to this course is not student.!");
            }
            _context.CourseUsers.Add(courseUser);
            _context.SaveChanges();
            return Ok(courseUser);
        }
    }
}