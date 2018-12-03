using evaBACKEND.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace evaBACKEND.Data
{
    public class AppDbContext : IdentityDbContext<AppUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<Course> Courses { get; set; }

        public DbSet<evaBACKEND.Models.Task> Tasks { get; set; }

		public DbSet<Grade> Grades { get; set; }

        public DbSet<CourseUser> CourseUsers { get; set; }

        public DbSet<Presentation> Presentations { get; set; }

		public DbSet<Test> Tests { get; set; }

		public DbSet<Question> Questions { get; set; }

		protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<CourseUser>()
                .HasKey(cu => new { cu.CourseId, cu.Id });

            builder.Entity<Presentation>().HasKey(p => new { p.StudentId, p.TaskId });
        }
    }
}
