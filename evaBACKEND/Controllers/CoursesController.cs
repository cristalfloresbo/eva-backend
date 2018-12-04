using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using evaBACKEND.Data;
using evaBACKEND.Models;
using Microsoft.AspNetCore.Identity;

namespace evaBACKEND.Controllers
{
    [Route("api/courses")]
    [ApiController]
    public class CoursesController : ControllerBase
    {
        private readonly AppDbContext _context;
		private readonly UserManager<AppUser> _userManager;

        public CoursesController(AppDbContext context, UserManager<AppUser> userManager)
        {
            _context = context;
			_userManager = userManager;
        }

        // GET: api/Courses
        [HttpGet]
        public IEnumerable<Course> GetCourses()
        {
            if (HttpContext.User.IsInRole("Docente"))
            {
                var userId = HttpContext.User.Claims.ElementAt(3).Value;
                var some = _context.Courses.Where(c => c.Teacher.Id == userId).ToList();
                return some;
            }
            return _context.Courses.Include(c => c.Teacher);
        }

        // GET: api/Courses/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetCourse([FromRoute] long id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var course = await _context.Courses.FindAsync(id);

            if (course == null)
            {
                return NotFound();
            }

            return Ok(course);
        }

        // PUT: api/Courses/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCourse([FromRoute] long id, [FromBody] Course course)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != course.CourseId)
            {
                return BadRequest();
            }

            _context.Entry(course).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CourseExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Courses
        [HttpPost]
        public async Task<IActionResult> PostCourse([FromBody] Course course)
        {
            if (!HttpContext.User.IsInRole("Docente"))
			{
				return Unauthorized();
			}

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var username = HttpContext.User.Claims.First().Value;
			AppUser teacher = await _userManager.FindByNameAsync(username);
			course.Teacher = teacher;
            _context.Courses.Add(course);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetCourse", new { id = course.CourseId }, course);
        }

        // DELETE: api/Courses/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCourse([FromRoute] long id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var course = await _context.Courses.FindAsync(id);
            if (course == null)
            {
                return NotFound();
            }

            _context.Courses.Remove(course);
            await _context.SaveChangesAsync();

            return Ok(course);
        }


        [HttpGet]
        [Route("{courseId}/students")]
        public IEnumerable<AppUser> GetAllStudentsOfACourse(long courseId)
        {
            var usersOfCourse = new List<AppUser>();
            var courseUsers = _context.CourseUsers.Where(cu => cu.CourseId == courseId).ToList();
            foreach (var courseUser in courseUsers)
            {
                var currentUser = _context.Users.Where(u => u.Id == courseUser.Id).First();
                usersOfCourse.Add(currentUser);
            }
            return usersOfCourse;
        }

        [HttpPost]
        [Route("students")]
        public IActionResult AddUserToCourse([FromBody] CourseUser courseUser)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userRole = _context.UserRoles.Where(ur => ur.UserId == courseUser.Id).First();
            var role = _context.Roles.Where(r => r.Id == userRole.RoleId).First();
            if (role.Name != "Estudiante")
            {
                return BadRequest("User pretended to add to this course is not student.!");
            }
            _context.CourseUsers.Add(courseUser);
            _context.SaveChanges();
            return Ok(courseUser);
        }

        private bool CourseExists(long id)
        {
            return _context.Courses.Any(e => e.CourseId == id);
        }
    }
}