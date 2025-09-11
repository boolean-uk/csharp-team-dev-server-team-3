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


            modelBuilder.Entity<UserCohort>()
                .HasKey(uc => new { uc.UserId, uc.CohortId });


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

            // seed users
            PersonData personData = new PersonData();
            PostData postData = new PostData(personData.Users);
            PostCommentData postCommentData = new PostCommentData(postData.Posts, personData.Users);
            modelBuilder.Entity<User>().HasData(personData.Users);
            modelBuilder.Entity<Post>().HasData(postData.Posts);
            modelBuilder.Entity<PostComment>().HasData(postCommentData.Comments);

        }
        public DbSet<User> Users { get; set; }
        public DbSet<Post> Posts { get; set; }
        public DbSet<PostComment> PostComments { get; set; }
        public DbSet<Cohort> Cohorts { get; set; }
        public DbSet<UserCohort> UserCohorts { get; set; }
    }
}
