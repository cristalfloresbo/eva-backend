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
    [Route("api/")]
    [ApiController]
    public class TasksController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly UserManager<AppUser> _userManager;

        public TasksController(AppDbContext context, UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: api/Tasks
        [HttpGet("tasks")]
        public IEnumerable<Models.Task> GetTasks()
        {
            return _context.Tasks;
        }

        // GET: api/Tasks/5
        [HttpGet("courses/{courseId}/tasks/{id}")]
        public async Task<IActionResult> GetTask(int courseId, [FromRoute] long id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var course = await _context.Courses.FindAsync((long)courseId);
            var task = await _context.Tasks.FindAsync(id);
            if (course == null && task == null)
            {
                return NotFound();
            }

            if (task.CourseId != course.CourseId)
            {
                return BadRequest("The task doesn't belong to the course");
            }

            var press = _context.Presentations.Where(p => p.Task.Equals(task)).ToList();
            task.Presentations = press;
            return Ok(task);
        }

        [HttpGet("courses/{id}/tasks")]
        public async Task<IActionResult> GetTasksByCourse([FromRoute] long id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var course = await _context.Courses.FindAsync(id);
            if (course ==null)
            {
                return NotFound("Course not found");
            }

            var tasks = _context.Tasks.Where(task => task.Course.Equals(course)).ToList();
            return Ok(course.Tasks);
        }

        // PUT: api/Tasks/5
        [HttpPut("courses/{courseId}/tasks/{id}")]
        public async Task<IActionResult> PutTask(int courseId, [FromRoute] long id, [FromBody] Models.Task task)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != task.ID)
            {
                return BadRequest();
            }

            _context.Entry(task).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TaskExists(id))
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

        // POST: api/Tasks
        [HttpPost("courses/{courseId}/tasks")]
        public async Task<IActionResult> PostTask(int courseId, [FromBody] Models.Task task)
        {
            if (!HttpContext.User.IsInRole("Docente"))
            {
                return Unauthorized();
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var course = await _context.Courses.FindAsync((long)courseId);
            if (course == null)
            {
                return NotFound();
            }
            var username = HttpContext.User.Claims.First().Value;
            AppUser teacher = await _userManager.FindByNameAsync(username);

            if (course.Teacher != teacher)
            {
                return Unauthorized();
            }

            task.Course = course;
            _context.Tasks.Add(task);
            course.Tasks.Add(task);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetTask", new { id = task.ID }, task);
        }

        // DELETE: api/Tasks/5
        [HttpDelete("courses/{courseId}/tasks/{id}")]
        public async Task<IActionResult> DeleteTask(int courseId, [FromRoute] long id)
        {
            if (!HttpContext.User.IsInRole("Docente"))
            {
                return Unauthorized();
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var course = await _context.Courses.FindAsync((long)courseId);
            var task = await _context.Tasks.FindAsync(id);
            if (course == null || task == null)
            {
                return NotFound();
            }

            if (task.CourseId != course.CourseId)
            {
                return BadRequest();
            }
            course.Tasks.Remove(task);
            _context.Tasks.Remove(task);
            await _context.SaveChangesAsync();

            return Ok(task);
        }

        private bool TaskExists(long id)
        {
            return _context.Tasks.Any(e => e.ID == id);
        }

        //Returns a  presentations by task and/or student
        [HttpGet("tasks/{taskId}/presentations/{studentdId?}")]
        public async Task<IActionResult> GetPresentationsByTask(int taskId, string studentdId = null)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Models.Task task;
            if (studentdId == null)
            {
                task = await _context.Tasks.FindAsync((long)taskId);
                var press = _context.Presentations.Where(p => p.Task.Equals(task)).ToList();
                return Ok(press);
            } else
            {
                task = await _context.Tasks.FindAsync(taskId);
                AppUser student = await _userManager.FindByIdAsync(studentdId);
                Presentation pres = await _context.Presentations.FindAsync((long)taskId, student.Id);
                pres.Task = task;
                pres.Student = student;
                return Ok(pres);
            }
        }

        //Terurns all the sent tasks of a student
        [HttpGet("my-tasks")]
        public async Task<IActionResult> GetPresentations()
        {
            if (!HttpContext.User.IsInRole("Estudiante"))
            {
                return Unauthorized();
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var username = HttpContext.User.Claims.First().Value;
            AppUser student = await _userManager.FindByEmailAsync(username);
            var pres = _context.Presentations.Where(p => p.StudentId.Equals(student.Id)).ToList();
            if (pres == null)
            {
                return NoContent();
            }

            return Ok(pres);
        }
    }
}