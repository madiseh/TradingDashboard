using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;

namespace TradingDashboard.Infrastructure.Persistence.SimulatedData.Repositories;

public class GenericQueryRepository<T, TKey> : IQueryRepository<T, TKey> where T : class
{
    private readonly DbContext _context;

    public GenericQueryRepository(DbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
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
}
