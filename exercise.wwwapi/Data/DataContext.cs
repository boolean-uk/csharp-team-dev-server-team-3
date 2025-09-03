using exercise.wwwapi.Configuration;
using exercise.wwwapi.Models;
using Microsoft.EntityFrameworkCore;
using System.Numerics;

namespace exercise.wwwapi.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {

        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            //optionsBuilder.UseInMemoryDatabase(databaseName: "Database");

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            modelBuilder.Entity<User>()
                .Property(u => u.Role)
                .HasConversion<string>();

            //modelBuilder.Entity<UserCohort>()
            //    .HasKey(tc => new { tc.UserId, tc.CohortId });

            //modelBuilder.Entity<UserCohort>()
            //    .HasOne(tc => tc.User)
            //    .WithMany(u => u.TeacherCohorts)
            //    .HasForeignKey(tc => tc.UserId);

            //modelBuilder.Entity<UserCohort>()
            //    .HasOne(tc => tc.Cohort)
            //    .WithMany(c => c.TeacherCohorts)
            //    .HasForeignKey(tc => tc.CohortId);

            // Seed Cohorts

            // seed users
            PersonData personData = new PersonData();
            modelBuilder.Entity<User>().HasData(personData.Users);

            // Seed UserCohorts
        }
        public DbSet<User> Users { get; set; }
    }
}
