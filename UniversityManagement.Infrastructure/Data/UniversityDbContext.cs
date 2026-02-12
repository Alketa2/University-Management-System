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
        public DbSet<Timetable> Timetables => Set<Timetable>();
        public DbSet<Announcement> Announcements => Set<Announcement>();
        public DbSet<Grade> Grades => Set<Grade>();
        public DbSet<StudentProgram> StudentPrograms => Set<StudentProgram>();
        //  Auth tables
        public DbSet<AppUser> Users => Set<AppUser>();
        public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // AppUser unique email
            modelBuilder.Entity<AppUser>()
                .HasIndex(u => u.Email)
                .IsUnique();

            // RefreshToken mapping
            modelBuilder.Entity<RefreshToken>()
                .HasKey(rt => rt.Id);

            modelBuilder.Entity<RefreshToken>()
                .HasIndex(rt => rt.Token)
                .IsUnique();

            modelBuilder.Entity<RefreshToken>()
                .HasOne(rt => rt.AppUser)
                .WithMany(u => u.RefreshTokens)
                .HasForeignKey(rt => rt.AppUserId)
                .OnDelete(DeleteBehavior.Cascade);

          
        }
    }
}
