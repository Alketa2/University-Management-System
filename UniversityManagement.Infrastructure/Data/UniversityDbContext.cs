using Microsoft.EntityFrameworkCore;
using UniversityManagement.Domain.Entities;

namespace UniversityManagement.Infrastructure.Data;

public class UniversityDbContext : DbContext
{
	public UniversityDbContext(DbContextOptions<UniversityDbContext> options) : base(options) { }

	public DbSet<Student> Students => Set<Student>();
	public DbSet<Teacher> Teachers => Set<Teacher>();
	public DbSet<Program> Programs => Set<Program>();
	public DbSet<Subject> Subjects => Set<Subject>();
	public DbSet<Exam> Exams => Set<Exam>();
	public DbSet<Attendance> Attendances => Set<Attendance>();
	public DbSet<Timetable> Timetables => Set<Timetable>();
	public DbSet<StudentProgram> StudentPrograms => Set<StudentProgram>();
	public DbSet<Announcement> Announcements => Set<Announcement>();

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		base.OnModelCreating(modelBuilder);

		// Unique indexes
		modelBuilder.Entity<Student>().HasIndex(x => x.Email).IsUnique();
		modelBuilder.Entity<Teacher>().HasIndex(x => x.Email).IsUnique();
		modelBuilder.Entity<Program>().HasIndex(x => x.Code).IsUnique();
		modelBuilder.Entity<Subject>().HasIndex(x => x.Code).IsUnique();

		// Avoid cascade cycles
		modelBuilder.Entity<Subject>()
			.HasOne(s => s.Program)
			.WithMany(p => p.Subjects)
			.HasForeignKey(s => s.ProgramId)
			.OnDelete(DeleteBehavior.Restrict);

		modelBuilder.Entity<Subject>()
			.HasOne(s => s.Teacher)
			.WithMany(t => t.Subjects)
			.HasForeignKey(s => s.TeacherId)
			.OnDelete(DeleteBehavior.Restrict);

		modelBuilder.Entity<Exam>()
			.HasOne(e => e.Subject)
			.WithMany(s => s.Exams)
			.HasForeignKey(e => e.SubjectId)
			.OnDelete(DeleteBehavior.Restrict);

		modelBuilder.Entity<Attendance>()
			.HasOne(a => a.Student)
			.WithMany(s => s.Attendances)
			.HasForeignKey(a => a.StudentId)
			.OnDelete(DeleteBehavior.Restrict);

		modelBuilder.Entity<Attendance>()
			.HasOne(a => a.Subject)
			.WithMany(s => s.Attendances)
			.HasForeignKey(a => a.SubjectId)
			.OnDelete(DeleteBehavior.Restrict);

		modelBuilder.Entity<Timetable>()
			.HasOne(t => t.Program)
			.WithMany(p => p.Timetables)
			.HasForeignKey(t => t.ProgramId)
			.OnDelete(DeleteBehavior.Restrict);

		modelBuilder.Entity<Timetable>()
			.HasOne(t => t.Subject)
			.WithMany(s => s.Timetables)
			.HasForeignKey(t => t.SubjectId)
			.OnDelete(DeleteBehavior.Restrict);

		modelBuilder.Entity<StudentProgram>()
			.HasOne(sp => sp.Student)
			.WithMany(s => s.StudentPrograms)
			.HasForeignKey(sp => sp.StudentId)
			.OnDelete(DeleteBehavior.Restrict);

		modelBuilder.Entity<StudentProgram>()
			.HasOne(sp => sp.Program)
			.WithMany(p => p.StudentPrograms)
			.HasForeignKey(sp => sp.ProgramId)
			.OnDelete(DeleteBehavior.Restrict);

		modelBuilder.Entity<StudentProgram>()
			.HasIndex(sp => new { sp.StudentId, sp.ProgramId })
			.IsUnique();
	}
}
