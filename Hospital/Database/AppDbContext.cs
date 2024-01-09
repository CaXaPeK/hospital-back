using Hospital.Database.TableModels;
using Microsoft.EntityFrameworkCore;
using System;

namespace Hospital.Database
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<BannedToken> BannedTokens { get; set; }

        public DbSet<Comment> Comments { get; set; }

        public DbSet<Consultation> Consultations { get; set; }

        public DbSet<Diagnosis> Diagnoses { get; set; }

        public DbSet<Doctor> Doctors { get; set; }

        public DbSet<Inspection> Inspections { get; set; }

        public DbSet<InspectionDiagnosis> InspectionDiagnoses { get; set; }

        public DbSet<Patient> Patients { get; set; }

        public DbSet<Speciality> Specialities { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Diagnosis>().HasIndex(x => x.Id);

            base.OnModelCreating(modelBuilder);
        }
    }
}
