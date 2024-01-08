using Hospital.Database.TableModels;
using Microsoft.EntityFrameworkCore;
using System;

namespace Hospital.Database
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<BannedToken> BannedTokens { get; set; }

        public DbSet<Diagnosis> Diagnoses { get; set; }

        public DbSet<Doctor> Doctors { get; set; }

        public DbSet<Speciality> Specialities { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Diagnosis>().HasIndex(x => x.Id);

            base.OnModelCreating(modelBuilder);
        }
    }
}
