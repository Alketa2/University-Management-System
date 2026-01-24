using UniversityManagement.Domain.Entities;
using UniversityManagement.Infrastructure.Data;

namespace UniversityManagement.Infrastructure.Repositories;

// Optional typed repository wrapper (generic IRepository<Program> is already registered)
public class ProgramRepository : EfRepository<Program>
{
    public ProgramRepository(UniversityDbContext db) : base(db) { }
}
