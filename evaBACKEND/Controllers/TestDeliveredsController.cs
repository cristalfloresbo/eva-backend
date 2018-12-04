using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using evaBACKEND.Data;
using evaBACKEND.Models;

namespace evaBACKEND.Controllers
{
    [Route("api/event/tests")]
    [ApiController]
    public class TestDeliveredsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public TestDeliveredsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/event/tests
        [HttpGet]
        public IEnumerable<TestDelivered> GetTestDelivered()
        {
            return _context.TestDelivered;
        }

        // GET: api/TestDelivereds/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetTestDelivered([FromRoute] long id)
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

            return Ok(testDelivered);
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
                if (!TestDeliveredExists(id))
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
                if (TestDeliveredExists(testDelivered.TestId))
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

        private bool TestDeliveredExists(long id)
        {
            return _context.TestDelivered.Any(e => e.TestId == id);
        }
    }
}