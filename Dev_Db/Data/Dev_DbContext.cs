using Dev_Models.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Dev_Db.Data
{
    public class Dev_DbContext : IdentityDbContext<IdentityUser>
    {

        public Dev_DbContext(DbContextOptions<Dev_DbContext> options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            //Course-Cart Relationship
            modelBuilder.Entity<Course>()
                .HasMany(s => s.carts)
                .WithMany(c => c.courses)
                .UsingEntity(j => j.ToTable("CartCourses"));

            //Course-Lesson Relationship
            modelBuilder.Entity<Lesson>()
                .HasOne(l => l.Course)
                .WithMany(c => c.Lessons)
                .HasForeignKey(l => l.CourseId);

            //Lesson-Video Relationship
            modelBuilder.Entity<Video>()
                .HasOne(v => v.Lesson)
                .WithMany(l => l.Videos)
                .HasForeignKey(v => v.LessonId);
        }



        public DbSet<User> Users { get; set; }

        public DbSet<Course> Courses { get; set; }

        public DbSet<Lesson> Lessons { get; set; }

        public DbSet<Video> Videos { get; set; }

        public DbSet<Cart> Carts { get; set; }



    }
}
