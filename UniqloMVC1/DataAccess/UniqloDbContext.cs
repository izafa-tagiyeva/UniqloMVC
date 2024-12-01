using Microsoft.EntityFrameworkCore;
using UniqloMVC1.Models;

//Why we use DataAcces instead of Models

namespace UniqloMVC1.DataAccess
{
    public class UniqloDbContext : DbContext
    {
        

        public DbSet<Slider> Sliders { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }
        public UniqloDbContext(DbContextOptions opt) :base(opt) { }

    }
}


