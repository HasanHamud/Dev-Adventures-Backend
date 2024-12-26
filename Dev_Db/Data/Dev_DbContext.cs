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

        public DbSet<User> Users { get; set; }


    }
}
