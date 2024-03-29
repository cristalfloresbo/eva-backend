﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using evaBACKEND.Data;
using evaBACKEND.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace evaBACKEND.Controllers
{
    [Route("api/event/tasks/")]
    [ApiController]
    public class TasksEventController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly UserManager<AppUser> _userManager;

        public TasksEventController(AppDbContext context, UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        //Enviar tarea como estudiante
        [HttpPost("{taskId}/deliver")]
        public async Task<IActionResult> PostTask([FromRoute] int taskId, [FromBody] Presentation presentation)
        {
            if (!HttpContext.User.IsInRole("Estudiante"))
            {
                return Unauthorized();
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var task = await _context.Tasks.FindAsync((long)taskId);
            var course = await _context.Courses.FindAsync(task.CourseId);
            if (task == null || course == null)
            {
                return NotFound();
            }

            var username = HttpContext.User.Claims.First().Value;
            AppUser student = await _userManager.FindByEmailAsync(username);

            presentation.StudentId = student.Id;
            presentation.Student = student;
            presentation.TaskId = taskId;
            presentation.Task = task;
            _context.Presentations.Add(presentation);
            task.Presentations.Add(presentation);

            await _context.SaveChangesAsync();

            return Ok(presentation);
        }

        //Returns a  presentation for the student
        [HttpGet("{id}/view")]
        public async Task<IActionResult> ViewDeliveredTask(int id)
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
            AppUser student = await _userManager.FindByNameAsync(username);
            Presentation pres = await _context.Presentations.FindAsync((long)id, student.Id);
            if (pres == null)
            {
                return NotFound();
            }

            return Ok(pres);
        }

        //Teacher can grade a students presentation
        [HttpPut("{taskId}/grade/{studentId}")]
        public async Task<IActionResult> GradeTask([FromRoute] int taskId, [FromRoute] string studentId, [FromBody] int gradeValue)
        {
            if (!HttpContext.User.IsInRole("Docente"))
            {
                return Unauthorized();
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var task = await _context.Tasks.FindAsync(taskId);
            var student = await _userManager.FindByIdAsync(studentId);
            Presentation pres = await _context.Presentations.FindAsync((long)taskId, studentId);
            if (pres == null)
            {
                return NotFound();
            }

            var username = HttpContext.User.Claims.First().Value;
            AppUser teacher = await _userManager.FindByNameAsync(username);
            if (!task.Course.Teacher.Equals(teacher))
            {
                return Unauthorized();
            }

            Grade grade = new Grade();
            grade.Student = student;
            grade.Value = gradeValue;
            grade.Course = task.Course;
            grade = _context.Grades.Add(grade).Entity;

            pres.Grade = grade;
            _context.Entry(pres).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}