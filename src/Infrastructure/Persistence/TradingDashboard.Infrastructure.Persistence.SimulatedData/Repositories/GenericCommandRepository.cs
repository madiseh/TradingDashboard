using Microsoft.EntityFrameworkCore;

namespace TradingDashboard.Infrastructure.Persistence.SimulatedData.Repositories;

public class GenericCommandRepository<T> : ICommandRepository<T> where T : class
{
    private readonly DbContext _context;

    public GenericCommandRepository(DbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public void Create(T entity)
    {
        ArgumentNullException.ThrowIfNull(entity);
        _context.Set<T>().Add(entity);
    }

    public void SaveChanges()
    {
        _context.SaveChanges();
    }
}
