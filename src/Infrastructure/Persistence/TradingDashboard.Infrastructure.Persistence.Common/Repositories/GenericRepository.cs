using System.Linq.Expressions;
using TradingDashboard.Application.Repository.Base;
using Microsoft.EntityFrameworkCore;

namespace TradingDashboard.Infrastructure.Persistence.Common.Repositories;

public class GenericRepository<T, TKey> : IRepository<T, TKey> where T : class
{
    protected readonly DbContext _context;

    public GenericRepository(DbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public void Create(T entity)
    {
        ArgumentNullException.ThrowIfNull(entity);
        _context.Set<T>().Add(entity);
    }

    public T Get(TKey id)
    {
        var entity = _context.Set<T>().Find(id);
        return entity ?? throw new KeyNotFoundException($"{typeof(T).Name} with key '{id}' was not found.");
    }

    public IList<T> GetList()
    {
        return _context.Set<T>().ToList();
    }

    public bool Exists(Expression<Func<T, bool>> expression)
    {
        ArgumentNullException.ThrowIfNull(expression);
        return _context.Set<T>().Any(expression);
    }

    public void SaveChanges()
    {
        _context.SaveChanges();
    }
}
