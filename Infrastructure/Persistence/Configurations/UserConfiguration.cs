using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JwtApi.Models;
using Microsoft.EntityFrameworkCore;
using SchoolApi.Models;

namespace Persistence.Configurations
{
    internal class UserConfiguration
    {
        public SchoolApiContext(DbContextOptions<SchoolApiContext> options)
            : base(options)
        {

        }
        public DbSet<Course> Courses { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<CourseStudent> CourseStudents { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<CourseStudent>()
                .HasKey(cs => new { cs.CourseId, cs.UserId });

            modelBuilder.Entity<CourseStudent>()
                .HasOne(cs => cs.Course)
                .WithMany(c => c.CourseStudents)
                .HasForeignKey(cs => cs.CourseId);

            modelBuilder.Entity<CourseStudent>()
                .HasOne(cs => cs.User)
                .WithMany(u => u.CourseStudents)
                .HasForeignKey(cs => cs.UserId);
        }
    }
}
