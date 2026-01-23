using Microsoft.EntityFrameworkCore;
using UniversityManagement.Domain.Common;
using UniversityManagement.Domain.Interfaces;
using UniversityManagement.Infrastructure.Data;

namespace UniversityManagement.Infrastructure.Repositories;

public class EfRepository<T> : IRepository<T> where T : BaseEntity
{
    protected readonly UniversityDbContext _db;
    protected readonly DbSet<T> _set;

    public EfRepository(UniversityDbContext db)
    {
        _db = db;
        _set = db.Set<T>();
    }

    public async Task<List<T>> GetAllAsync()
        => await _set.AsNoTracking().ToListAsync();

    public async Task<T?> GetByIdAsync(Guid id)
        => await _set.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);

    public async Task<T> AddAsync(T entity)
    {
        if (entity.Id == Guid.Empty)
            entity.Id = Guid.NewGuid();

        entity.CreatedAt = DateTime.UtcNow;
        _set.Add(entity);
        await _db.SaveChangesAsync();
        return entity;
    }

    public async Task<T> UpdateAsync(T entity)
    {
        entity.UpdatedAt = DateTime.UtcNow;
        _set.Update(entity);
        await _db.SaveChangesAsync();
        return entity;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var entity = await _set.FirstOrDefaultAsync(x => x.Id == id);
        if (entity is null) return false;

        _set.Remove(entity);
        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ExistsAsync(Guid id)
        => await _set.AnyAsync(x => x.Id == id);
}
