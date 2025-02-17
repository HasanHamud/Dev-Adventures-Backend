using Dev_Models.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Dev_Db.Data
{
    public class Dev_DbContext : IdentityDbContext<User>
    {
        public Dev_DbContext(DbContextOptions<Dev_DbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Course-Cart Relationship
            modelBuilder.Entity<Course>()
                .HasMany(s => s.carts)
                .WithMany(c => c.courses)
                .UsingEntity(j => j.ToTable("CartCourses"));

            // Course-Lesson Relationship
            modelBuilder.Entity<Lesson>()
                .HasOne(l => l.Course)
                .WithMany(c => c.Lessons)
                .HasForeignKey(l => l.CourseId);

            // Lesson-Video Relationship
            modelBuilder.Entity<Video>()
                .HasOne(v => v.Lesson)
                .WithMany(l => l.Videos)
                .HasForeignKey(v => v.LessonId);

            // Course-Requirement Relationship
            modelBuilder.Entity<CourseRequirement>()
                .HasOne(r => r.Course)
                .WithMany(c => c.Requirements)
                .HasForeignKey(r => r.CourseId);

            // Course-LearningObjective Relationship
            modelBuilder.Entity<CourseLearningObjective>()
                .HasOne(o => o.Course)
                .WithMany(c => c.LearningObjectives)
                .HasForeignKey(o => o.CourseId);

            // ChatMessage Relationships
            modelBuilder.Entity<ChatMessage>()
                .HasOne(m => m.Sender)
                .WithMany()
                .HasForeignKey(m => m.SenderId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ChatMessage>()
                .HasOne(m => m.Receiver)
                .WithMany()
                .HasForeignKey(m => m.ReceiverId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<UserCourse>()
            .HasKey(uc => new { uc.UserId, uc.CourseId });

            modelBuilder.Entity<UserCourse>()
                .HasOne(uc => uc.User)
                .WithMany(u => u.UserCourses)
                .HasForeignKey(uc => uc.UserId);

            modelBuilder.Entity<UserCourse>()
                .HasOne(uc => uc.Course)
                .WithMany(c => c.UserCourses)
                .HasForeignKey(uc => uc.CourseId);
        }

        public DbSet<Course> Courses { get; set; }
        public DbSet<Lesson> Lessons { get; set; }
        public DbSet<Video> Videos { get; set; }
        public DbSet<Cart> Carts { get; set; }
        public DbSet<CourseRequirement> CourseRequirements { get; set; }
        public DbSet<CourseLearningObjective> CourseLearningObjectives { get; set; }
        public DbSet<ChatMessage> ChatMessages { get; set; }
        public DbSet<UserCourse> UserCourses { get; set; }

        public DbSet<Coupon> Coupons { get; set; }
    }
}