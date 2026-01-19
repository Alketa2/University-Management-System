using UniversityManagement.Domain.Common;    
using UniversityManagement.Domain.Interfaces; 
using UniversityManagement.Domain.Entities;   


namespace UniversityManagement.Infrastructure.Repositories;

public class Repository<T> : IRepository<T> where T : BaseEntity
{
    protected readonly Dictionary<Guid, T> _entities = new();

    public Task<T?> GetByIdAsync(Guid id)
    {
        _entities.TryGetValue(id, out var entity);
        return Task.FromResult<T?>(entity);
    }

    public Task<List<T>> GetAllAsync()
    {
        return Task.FromResult(_entities.Values.ToList());
    }

    public Task<T> AddAsync(T entity)
    {
        if (entity.Id == Guid.Empty)
        {
            entity.Id = Guid.NewGuid();
        }
        entity.CreatedAt = DateTime.UtcNow;
        _entities[entity.Id] = entity;
        return Task.FromResult(entity);
    }

    public Task<T> UpdateAsync(T entity)
    {
        entity.UpdatedAt = DateTime.UtcNow;
        _entities[entity.Id] = entity;
        return Task.FromResult(entity);
    }

    public Task<bool> DeleteAsync(Guid id)
    {
        return Task.FromResult(_entities.Remove(id));
    }

    public Task<bool> ExistsAsync(Guid id)
    {
        return Task.FromResult(_entities.ContainsKey(id));
    }
}
