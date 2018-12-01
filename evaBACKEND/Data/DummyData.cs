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

		}
	}

			
}
