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
    }
}