using exercise.wwwapi.Models;
using Microsoft.EntityFrameworkCore;

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
            #region CohortCourse
            // Composite key for CohortCourse
            modelBuilder.Entity<CohortCourse>()
                .HasKey(cc => new { cc.CohortId, cc.CourseId });

            modelBuilder.Entity<CohortCourseUser>()
                .HasKey(ccu => new { ccu.CohortId, ccu.CourseId, ccu.UserId });

            // Relationships
            modelBuilder.Entity<CohortCourse>()
                .HasOne(cc => cc.Cohort)
                .WithMany(c => c.CohortCourses)
                .HasForeignKey(cc => cc.CohortId);

            modelBuilder.Entity<CohortCourse>()
                .HasOne(cc => cc.Course)
                .WithMany(c => c.CohortCourses)
                .HasForeignKey(cc => cc.CourseId);

            modelBuilder.Entity<CohortCourseUser>()
                .HasOne(ccu => ccu.Cohort)
                .WithMany()  // ← Specify the inverse navigation
                .HasForeignKey(ccu => ccu.CohortId);

            modelBuilder.Entity<CohortCourseUser>()
                .HasOne(ccu => ccu.Course)
                .WithMany()  // ← Specify the inverse navigation
                .HasForeignKey(ccu => ccu.CourseId);

            modelBuilder.Entity<CohortCourseUser>()
                .HasOne(ccu => ccu.User)
                .WithMany(u => u.CohortCourseUsers)  // ← Specify the inverse navigation
                .HasForeignKey(ccu => ccu.UserId);
            #endregion CohortCourse

            modelBuilder.Entity<UserCohort>()
                .HasKey(uc => new { uc.UserId, uc.CohortId });


            modelBuilder.Entity<User>()
                .Property(u => u.Role)
                .HasConversion<string>();

            modelBuilder.Entity<Post>()
                .HasMany(p => p.Comments)
                .WithOne(c => c.Post)
                .HasForeignKey(c => c.PostId)
                .OnDelete(DeleteBehavior.Cascade);

            // seed users
            PersonData personData = new PersonData();
            // seed cohorts with courses
            CohortCourseData cohortCourseData = new CohortCourseData(personData.Users);
            // seed posts
            PostData postData = new PostData(personData.Users);
            PostCommentData postCommentData = new PostCommentData(postData.Posts, personData.Users);
            modelBuilder.Entity<User>().HasData(personData.Users);
            modelBuilder.Entity<Post>().HasData(postData.Posts);
            modelBuilder.Entity<PostComment>().HasData(postCommentData.Comments);
            modelBuilder.Entity<Course>().HasData(cohortCourseData.Courses);
            modelBuilder.Entity<Cohort>().HasData(cohortCourseData.Cohorts);
            modelBuilder.Entity<CohortCourse>().HasData(cohortCourseData.CohortCourses);
            modelBuilder.Entity<CohortCourseUser>().HasData(cohortCourseData.CohortCourseUsers);

        }
        public DbSet<User> Users { get; set; }
        public DbSet<Post> Posts { get; set; }
        public DbSet<PostComment> PostComments { get; set; }
        public DbSet<Cohort> Cohorts { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<CohortCourse> CohortCourses { get; set; }
        public DbSet<CohortCourseUser> CohortCourseUsers { get; set; }
        public DbSet<UserCohort> UserCohorts { get; set; }
    }
}
