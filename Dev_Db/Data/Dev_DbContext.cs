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

            modelBuilder.Entity<Quiz>()
                .HasOne(q => q.Lesson)
                .WithOne(l => l.Quiz)
                .HasForeignKey<Quiz>(q => q.LessonId);

            modelBuilder.Entity<QuizQuestion>()
                .HasOne(q => q.Quiz)
                .WithMany(qz => qz.Questions)
                .HasForeignKey(q => q.QuizId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<QuizAnswer>()
                .HasOne(a => a.Question)
                .WithMany(q => q.Answers)
                .HasForeignKey(a => a.QuestionId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<UserQuizResult>()
                .HasOne(r => r.User)
                .WithMany()
                .HasForeignKey(r => r.UserId);

            modelBuilder.Entity<UserQuizResult>()
                .HasOne(r => r.Quiz)
                .WithMany()
                .HasForeignKey(r => r.QuizId);

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

            // Plan-Cart Relationship
            modelBuilder.Entity<PlansCarts>(entity =>
            {
                entity.ToTable("PlansCarts");
                entity.HasOne(pc => pc.Plan)
                      .WithMany(p => p.PlansCarts)
                      .HasForeignKey(pc => pc.PlanId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(pc => pc.Cart)
                      .WithMany(c => c.PlansCarts)
                      .HasForeignKey(pc => pc.CartId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasIndex(pc => new { pc.PlanId, pc.CartId });
            });

            // Plan-Course Relationship
            modelBuilder.Entity<PlansCourses>(entity =>
            {
                entity.ToTable("PlansCourses");
                entity.HasOne(pc => pc.Plan)
                      .WithMany(p => p.PlansCourses)
                      .HasForeignKey(pc => pc.PlanId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(pc => pc.Course)
                      .WithMany(c => c.PlansCourses)
                      .HasForeignKey(pc => pc.CourseId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasIndex(pc => new { pc.PlanId, pc.CourseId });
            });

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
        public DbSet<Quiz> Quizzes { get; set; }
        public DbSet<QuizQuestion> QuizQuestions { get; set; }
        public DbSet<QuizAnswer> QuizAnswers { get; set; }
        public DbSet<UserQuizResult> UserQuizResults { get; set; }
        public DbSet<ChatMessage> ChatMessages { get; set; }
        public DbSet<Plan> Plans { get; set; }
        public DbSet<PlansCourses> PlansCourses { get; set; }
        public DbSet<PlansCarts> PlansCarts { get; set; }
        public DbSet<Coupon> Coupons { get; set; }
        public DbSet<UserCourse> UserCourses { get; set; }
    }
}
