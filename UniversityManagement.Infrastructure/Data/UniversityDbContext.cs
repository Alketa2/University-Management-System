using Microsoft.EntityFrameworkCore;
using UniversityManagement.Domain.Entities;

namespace UniversityManagement.Infrastructure.Data
{
    public class UniversityDbContext : DbContext
    {
        public UniversityDbContext(DbContextOptions<UniversityDbContext> options)
            : base(options) { }

        public DbSet<Student> Students => Set<Student>();
        public DbSet<Teacher> Teachers => Set<Teacher>();
        public DbSet<Subject> Subjects => Set<Subject>();
        public DbSet<Program> Programs => Set<Program>();
        public DbSet<Exam> Exams => Set<Exam>();
        public DbSet<Attendance> Attendances => Set<Attendance>();
        public DbSet<Timetable> Timetables => Set<Timetable>();
        public DbSet<Announcement> Announcements => Set<Announcement>();
        public DbSet<StudentProgram> StudentPrograms => Set<StudentProgram>();
        public DbSet<AppUser> Users => Set<AppUser>();
        public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Subject -> Program
            modelBuilder.Entity<Subject>()
                .HasOne(s => s.Program)
                .WithMany(p => p.Subjects)
                .HasForeignKey(s => s.ProgramId)
                .OnDelete(DeleteBehavior.Restrict);

            // Subject -> Teacher
            modelBuilder.Entity<Subject>()
                .HasOne(s => s.Teacher)
                .WithMany(t => t.Subjects)
                .HasForeignKey(s => s.TeacherId)
                .OnDelete(DeleteBehavior.Restrict);

            // Student <-> Program (many-to-many via StudentProgram)
            modelBuilder.Entity<StudentProgram>()
                .HasKey(sp => new { sp.StudentId, sp.ProgramId });

            modelBuilder.Entity<StudentProgram>()
                .HasOne(sp => sp.Student)
                .WithMany(s => s.StudentPrograms)
                .HasForeignKey(sp => sp.StudentId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<StudentProgram>()
                .HasOne(sp => sp.Program)
                .WithMany(p => p.StudentPrograms)
                .HasForeignKey(sp => sp.ProgramId)
                .OnDelete(DeleteBehavior.Cascade);

            // Timetable -> Program
            modelBuilder.Entity<Timetable>()
                .HasOne(t => t.Program)
                .WithMany(p => p.Timetables)
                .HasForeignKey(t => t.ProgramId)
                .OnDelete(DeleteBehavior.Cascade);

            // Timetable -> Subject
            modelBuilder.Entity<Timetable>()
                .HasOne(t => t.Subject)
                .WithMany(s => s.Timetables)
                .HasForeignKey(t => t.SubjectId)
                .OnDelete(DeleteBehavior.Cascade);

            // Announcement -> Teacher (allow null if Teacher deleted)
            modelBuilder.Entity<Announcement>()
                .HasOne(a => a.Teacher)
                .WithMany(t => t.Announcements)
                .HasForeignKey(a => a.TeacherId)
                .OnDelete(DeleteBehavior.SetNull);

            // Announcement -> Program (if you have ProgramId in Announcement)
            modelBuilder.Entity<Announcement>()
                .HasOne(a => a.Program)
                .WithMany(p => p.Announcements)
                .HasForeignKey(a => a.ProgramId)
                .OnDelete(DeleteBehavior.Cascade);

            // AppUser email unique
            modelBuilder.Entity<AppUser>()
                .HasIndex(u => u.Email)
                .IsUnique();

            // AppUser -> RefreshTokens (1-to-many)
            modelBuilder.Entity<RefreshToken>()
                .HasOne(rt => rt.AppUser)
                .WithMany(u => u.RefreshTokens)
                .HasForeignKey(rt => rt.AppUserId)
                .OnDelete(DeleteBehavior.Cascade);

            // RefreshToken token unique (optional but recommended)
            modelBuilder.Entity<RefreshToken>()
                .HasIndex(rt => rt.Token)
                .IsUnique();

        }
    }
}
