using System.Linq.Expressions;

namespace TradingDashboard.Application.Repository.Base;

public interface IRepository<T, in TKey> where T : class
{
    void Create(T entity);
    T Get(TKey id);
    IList<T> GetList();
    bool Exists(Expression<Func<T, bool>> expression);
    void SaveChanges();
}
