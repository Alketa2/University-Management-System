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
	}
}
