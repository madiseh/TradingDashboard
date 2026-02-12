namespace TradingDashboard.Application.Repository.Base;

public interface ICommandRepository<T> where T : class
{
    void Create(T entity);
    void SaveChanges();
}
