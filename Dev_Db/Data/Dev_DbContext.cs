using Dev_Models;
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

            modelBuilder.Entity<Course>()
                .HasMany(s => s.carts) 
                .WithMany(c => c.courses) 
                .UsingEntity(j => j.ToTable("CartCourses"));  
        }


        public DbSet<User> Users { get; set; }
        
        public DbSet<Course> Courses { get; set; }

        public DbSet<Cart> Carts { get; set; }


    }
}
