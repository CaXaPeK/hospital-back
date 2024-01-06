using Hospital.Database.TableModels;
using Microsoft.EntityFrameworkCore;
using System;

namespace Hospital.Database
{
    public class IcdDbContext : DbContext
    {
        public IcdDbContext(DbContextOptions<IcdDbContext> options) : base(options) { }

        public DbSet<Diagnosis> Diagnoses { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Diagnosis>().HasIndex(x => x.Id);

            base.OnModelCreating(modelBuilder);
        }
    }
}
