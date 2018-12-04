using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using evaBACKEND.Data;
using evaBACKEND.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace evaBACKEND.Controllers
{
    [Route("api/")]
    [ApiController]
    public class TestsController : ControllerBase
    {
        private readonly AppDbContext _context;
		private readonly UserManager<AppUser> _userManager;

        public TestsController(AppDbContext context, UserManager<AppUser> userManager)
        {
            _context = context;
			_userManager = userManager;
        }

        // GET: api/Tests
        [HttpGet("tests")]
        public IEnumerable<Test> GetTests()
        {
            return _context.Tests;
        }

        // GET: api/tests/5/questions
        [HttpGet("tests/{id}/questions")]

        public async Task<IActionResult> GetTest([FromRoute] long id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var test = await _context.Tests.FindAsync(id);

            if (test == null)
            {
                return NotFound();
            }

			var questions = _context.Questions.Where(question => question.Test.Equals(test)).ToList();
			var questionsToResponse = new List<Object>();
			 
			foreach (var q in questions)
			{
				var answersByQuestion = _context.Answers.Where(answer => answer.Question.Equals(q)).ToList();
				questionsToResponse.Add(new {
					question = q,
					answers = answersByQuestion
				});
			}

            return Ok(questionsToResponse);
        }

		// GET: api/courses/5/tests
		[HttpGet("courses/{id}/tests")]
		public async Task<IActionResult> GetTestsByCourse([FromRoute] long id)
		{
			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}

			var couse = await _context.Courses.FindAsync(id);

			if (couse == null)
			{
				return NotFound();
			}

			var tests = _context.Tests.Where(test => test.Course.Equals(couse)).ToList();

			return Ok(tests);
		}

		// PUT: api/Tests/5
		[HttpPut("tests/{id}")]
        public async Task<IActionResult> PutTest([FromRoute] long id, [FromBody] Test test)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != test.TestId)
            {
                return BadRequest();
            }

            _context.Entry(test).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TestExists(id))
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

        // POST: api/Tests
        [HttpPost("courses/{courseId}/tests")]
		[Authorize(AuthenticationSchemes ="Bearer", Roles = "Admin, Docente")]
        public async Task<IActionResult> PostTest([FromRoute] long courseId, [FromBody] Test test)
        {
			Course course = await _context.Courses.FindAsync(courseId);

			if (course == null) {
				return NotFound("Course not found");
			}

			var username = HttpContext.User.Claims.First().Value;
			AppUser teacher = await _userManager.FindByNameAsync(username);

			if (!course.Teacher.Equals(teacher)) {
				return Unauthorized();
			}

			Test testToCreate = new Test();
			testToCreate.Title = test.Title;
			testToCreate.Course = course;
			testToCreate.Date = test.Date;
			testToCreate.Description = test.Description;

            _context.Tests.Add(testToCreate);
            await _context.SaveChangesAsync();

			ICollection<Question> questions = test.Questions;

			foreach (var question in questions) {
				question.Test = testToCreate;
			    _context.Questions.Add(question);
				await _context.SaveChangesAsync();
				var answers = question.Answers;
				foreach (var answer in answers) {
					answer.Question = question;
					_context.Answers.Add(answer);
					await _context.SaveChangesAsync();
				}
				
			}

            return CreatedAtAction("GetTest", new { id = test.TestId }, testToCreate);
        }

        // DELETE: api/Tests/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTest([FromRoute] long id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var test = await _context.Tests.FindAsync(id);
            if (test == null)
            {
                return NotFound();
            }

            _context.Tests.Remove(test);
            await _context.SaveChangesAsync();

            return Ok(test);
        }

        private bool TestExists(long id)
        {
            return _context.Tests.Any(e => e.TestId == id);
        }
    }
}