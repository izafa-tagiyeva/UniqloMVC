using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using UniqloMVC1.Models;



namespace UniqloMVC1.DataAccess
{
    public class UniqloDbContext : IdentityDbContext<User>
    {
        

        public DbSet<Slider> Sliders { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }
        public UniqloDbContext(DbContextOptions opt) :base(opt) { }

    }
}


