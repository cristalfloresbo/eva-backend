using evaBACKEND.Models;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Task = System.Threading.Tasks.Task;

namespace evaBACKEND.Data
{
	public class DummyData
	{
		public static async Task Initialize(AppDbContext context,
						  RoleManager<IdentityRole> roleManager,
						  UserManager<AppUser> userManager)
		{
			context.Database.EnsureCreated();

			string[] roles = { "Admin", "Docente", "Estudiante" };

			foreach (string role in roles)
			{
				if (await roleManager.FindByNameAsync(role) == null)
				{
					await roleManager.CreateAsync(new IdentityRole(role));
				}
			}

			var adminUser = new AppUser();
			adminUser.FirstName = "Admin";
			adminUser.LastName = "Admin";
			adminUser.Email = "admin@mail.com";
			adminUser.UserName = "admin@mail.com";

			if (await userManager.FindByEmailAsync(adminUser.Email) == null) {
				await userManager.CreateAsync(adminUser, "Control123!");
				await userManager.AddToRoleAsync(adminUser, "Admin");
			}

			var teacherUser = new AppUser();
			teacherUser.FirstName = "Pablo";
			teacherUser.LastName = "Gonzales";
			teacherUser.Email = "pablo.gz@mail.com";
			teacherUser.UserName = "pablo.gz@mail.com";

			if (await userManager.FindByEmailAsync(teacherUser.Email) == null)
			{
				await userManager.CreateAsync(teacherUser, "Control123!");
				await userManager.AddToRoleAsync(teacherUser, "Docente");
			}

			var javaCourse = new Course();
			javaCourse.Name = "Java para principiantes";
			javaCourse.Description = "Conceptos basicos de uno de lenguajes mas utilizados al rededor del mundo," +
				"en el cuerso aprenderas todo lo relicionado con POO";
			javaCourse.StartDate = DateTime.Today;
			javaCourse.Teacher = teacherUser;

			var existCourse = context.Courses.Where(c => c.Name.Equals(javaCourse.Name)).Count();

			if (existCourse == 0)
			{
				context.Courses.Add(javaCourse);
				await context.SaveChangesAsync();
			}

			var csharpCourse = new Course();
			csharpCourse.Name = "Programación en C#";
			csharpCourse.Description = "En este curso aprenderás a programar desde cero. No necesitarás " +
				"tener conocimientos previos en programación para aprender." +
				"Empezaremos por lo más básico, desde instalar el compilador donde trabajar, pasando por la " +
				"realización de pequeños programas de consola, hasta la programación orientada a objetos.";
			csharpCourse.Teacher = teacherUser;
			csharpCourse.StartDate = new DateTime(2019, 1, 3, 16, 5, 7, 123);

			existCourse = context.Courses.Where(c => c.Name.Equals(csharpCourse.Name)).Count();

			if (existCourse == 0) {
				context.Courses.Add(csharpCourse);
				await context.SaveChangesAsync();
			}
			

			var student = new AppUser();
			student.FirstName = "Andrea";
			student.LastName = "Panozo";
			student.Email = "andy24@mail.com";
			student.UserName = "andy24@mail.com";

			if (await userManager.FindByEmailAsync(student.Email) == null)
			{
				await userManager.CreateAsync(student, "Control123!");
				await userManager.AddToRoleAsync(student, "Estudiante");
			}

			var courseUser = new CourseUser();
			courseUser.CourseId = javaCourse.CourseId;
			courseUser.Id = student.Id;

			context.CourseUsers.Add(courseUser);
			await context.SaveChangesAsync();
		}
	}
		
}
