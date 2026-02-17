using System.Linq.Expressions;

namespace TradingDashboard.Application.Repository.Base;

public interface IQueryRepository<T, in TKey> where T : class
{
    T Get(TKey id);
    IList<T> GetList();
    bool Exists(Expression<Func<T, bool>> expression);
}
