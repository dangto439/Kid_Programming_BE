using KidPrograming.Entity;
using Microsoft.EntityFrameworkCore;



namespace KidPrograming.Repositories.Base
{
    public class KidProgramingDbContext : DbContext
    {
        public KidProgramingDbContext(DbContextOptions<KidProgramingDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }

        public DbSet<Course> Courses { get; set; }
        public DbSet<Chapter> Chapters { get; set; }
        public DbSet<Lesson> Lessons { get; set; }
        public DbSet<ChapterProgress> ChapterProgresses { get; set; }
        public DbSet<Lab> Labs { get; set; }
        public DbSet<Enrollment> Enrollments { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<Submission> Submissions { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Enrollment>()
                .HasOne(e => e.User)
                .WithMany(u => u.Enrollments)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Enrollment>()
                .HasOne(e => e.Course)
                .WithMany(c => c.Enrollments)
                .HasForeignKey(e => e.CourseId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<User>()
                .HasOne(u => u.Parent)
                .WithMany(u => u.Children)
                .HasForeignKey(u => u.ParentId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Payment>()
                .HasOne(p => p.User)
                .WithMany(u => u.Payments)
                .HasForeignKey(p => p.UserId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Payment>()
                .HasOne(p => p.Course)
                .WithMany(c => c.Payments)
                .HasForeignKey(p => p.CourseId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ChapterProgress>()
                .HasOne(cp => cp.Enrollment)
                .WithMany(e => e.ChapterProgresses)
                .HasForeignKey(cp => cp.EnrollmentId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<ChapterProgress>()
                .HasOne(cp => cp.Chapter)
                .WithMany(c => c.ChapterProgresses)
                .HasForeignKey(cp => cp.ChapterId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Submission>()
                .HasOne(s => s.User)
                .WithMany(u => u.Submissions)
                .HasForeignKey(s => s.UserId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Submission>()
                .HasOne(s => s.Lab)
                .WithMany()
                .HasForeignKey(s => s.LabId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Submission>()
                .HasOne(s => s.ChapterProgress)
                .WithMany(cp => cp.Submissions)
                .HasForeignKey(s => s.ChapterProgressId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Notification>()
                .HasOne(n => n.User)
                .WithMany(u => u.Notifications)
                .HasForeignKey(n => n.ReceiverId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Lab>()
                .HasOne(l => l.Lesson)
                .WithMany()
                .HasForeignKey(l => l.LessionId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Course>()
               .HasOne(c => c.Teacher)
               .WithMany(u => u.Courses)
               .HasForeignKey(c => c.TeacherId)
               .OnDelete(DeleteBehavior.NoAction);

            base.OnModelCreating(modelBuilder);
        }

    }
}

