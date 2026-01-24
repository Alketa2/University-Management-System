using System.Collections.Concurrent;
using UniversityManagement.Domain.Common;
using UniversityManagement.Domain.Interfaces;

namespace UniversityManagement.Infrastructure.Repositories;

/// <summary>
/// In-memory repository used during Step 1 (no DB connections yet).
/// 
/// NOTE: This is registered as a singleton so data persists across requests while testing in Swagger.
/// When you switch to EF Core/Mongo, replace this with real persistence and switch lifetimes back to scoped.
/// </summary>
public class Repository<T> : IRepository<T> where T : BaseEntity
{
    // Thread-safe so we can safely use singleton lifetime while Swagger testing.
    protected readonly ConcurrentDictionary<Guid, T> _entities = new();

    public Task<T?> GetByIdAsync(Guid id)
    {
        _entities.TryGetValue(id, out var entity);
        return Task.FromResult(entity);
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
        return Task.FromResult(_entities.TryRemove(id, out _));
    }

    public Task<bool> ExistsAsync(Guid id)
    {
        return Task.FromResult(_entities.ContainsKey(id));
    }
}
