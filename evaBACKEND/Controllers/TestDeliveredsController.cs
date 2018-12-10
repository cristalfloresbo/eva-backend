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
    [Route("api/event/tests")]
    [ApiController]
    public class TestDeliveredsController : ControllerBase
    {
        private readonly AppDbContext _context;
		private readonly UserManager<AppUser> _userManager;


		public TestDeliveredsController(AppDbContext context, UserManager<AppUser> userManager)
        {
            _context = context;
			_userManager = userManager;
        }

        // GET: api/event/tests
        [HttpGet]
        public IEnumerable<TestDelivered> GetTestDelivered()
        {
            return _context.TestDelivered;
        }

        // GET: api/event/tests/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetTestDelivered([FromRoute] long id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

			var toResponse = new List<Object>();
            var testDelivered =  _context.TestDelivered.Where(ts => ts.TestId == id).ToList();
			foreach (var test in testDelivered)
			{
				var student = await _userManager.FindByIdAsync(test.StudentId);
				test.Student = student;
				//var answer = await _context.AnswerDelivered
				//	.Where(aw => aw.TestDelivered.StudentId == test.StudentId && aw.TestDelivered.TestId == id).ToListAsync();
				//toResponse.Add(new
				//{
				//	testdeliver = test,
				//	answers = answer
				//});
			}



			if (testDelivered == null)
            {
                return NotFound();
            }

            return Ok(testDelivered);
        }

		// GET: api/event/tests/5
		[HttpGet("{id}/students/{studentId}")]
		public async Task<IActionResult> GetTestDeliveredByStudent([FromRoute] long id,
			[FromRoute] string studentId)
		{
			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}

			var toResponse = new Object();
			var toAnswers = new List<Object>();

			var answers = await _context.AnswerDelivered
				.Where(aw => aw.TestDelivered.StudentId == studentId && aw.TestDelivered.TestId == id).ToListAsync();

			foreach (var delivered in answers) {
				var questionFinded = await _context.Questions.FindAsync(delivered.QuestionId);
				var answerFinded = await _context.Answers.FindAsync(delivered.AnswerId);
				toAnswers.Add(new {
					question = questionFinded,
				    answer = answerFinded
				});
			}
			toResponse = new
			{
				answers = toAnswers
			};

			return Ok(toResponse);
		}

		// PUT: api/TestDelivereds/5
		[HttpPut("{id}")]
        public async Task<IActionResult> PutTestDelivered([FromRoute] long id, [FromBody] TestDelivered testDelivered)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != testDelivered.TestId)
            {
                return BadRequest();
            }

            _context.Entry(testDelivered).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TestDeliveredExists(id,""))
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

        // POST: api/TestDelivereds
        [HttpPost("{testId}/deliver")]
        public async Task<IActionResult> PostTestDelivered([FromRoute] long testId,[FromBody] TestDelivered testDelivered)
        {
			var userId = HttpContext.User.Claims.ElementAt(3).Value;


			testDelivered.StudentId = userId;
			testDelivered.TestId = testId;

			_context.TestDelivered.Add(testDelivered);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (TestDeliveredExists(testDelivered.TestId, userId))
                {
                    return new StatusCodeResult(StatusCodes.Status409Conflict);
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetTestDelivered", new { id = testDelivered.TestId }, testDelivered);
        }

        // DELETE: api/TestDelivereds/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTestDelivered([FromRoute] long id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var testDelivered = await _context.TestDelivered.FindAsync(id);
            if (testDelivered == null)
            {
                return NotFound();
            }

            _context.TestDelivered.Remove(testDelivered);
            await _context.SaveChangesAsync();

            return Ok(testDelivered);
        }

        private bool TestDeliveredExists(long id, string studentId)
        {
            return _context.TestDelivered.Any(e => e.TestId == id && e.StudentId.Equals(studentId));
        }
    }
}