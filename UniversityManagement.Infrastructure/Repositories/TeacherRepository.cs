using UniversityManagement.Domain.Entities;
using UniversityManagement.Infrastructure.Data;

namespace UniversityManagement.Infrastructure.Repositories;

// Optional typed repository wrapper (generic IRepository<Teacher> is already registered)
public class TeacherRepository : EfRepository<Teacher>
{
    public TeacherRepository(UniversityDbContext db) : base(db) { }
}
