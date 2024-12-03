using Microsoft.EntityFrameworkCore;
using PetAdoptionApp.Models;

namespace PetAdoptionApp.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Pet> Pets { get; set; }
        public DbSet<Tag> Tags { get; set; }  // Added Tags DbSet

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configure many-to-many relationship between Pets and Tags
            modelBuilder.Entity<Pet>()
                .HasMany(p => p.Tags)
                .WithMany(t => t.Pets);

            base.OnModelCreating(modelBuilder);
        }
    }
}
