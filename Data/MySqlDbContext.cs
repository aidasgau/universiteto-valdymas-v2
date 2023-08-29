using Microsoft.EntityFrameworkCore;
using project_mvc.Models.Domain;

namespace project_mvc.Data
{
    public class MySqlDbContext : DbContext
    {
        public MySqlDbContext(DbContextOptions options) : base(options)
        {

        }

        public DbSet<Student> Students { get; set; }
        public DbSet<Instructor> Instructors { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<Enrollment> Enrollments { get; set; }
        public DbSet<Grade> Grades { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Grade>().HasKey(g => g.EnrollmentID);
        }
    }
}
