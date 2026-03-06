

namespace ECommerce.Domain.Contracts
{
    public interface IUnitOfWork
    {
 
        Task<int> SaveChangesAsync();
        IGenericRepository<TEntity,TKey> GetRepository<TEntity, TKey>() where TEntity : BaseEnitiy<TKey>;

    }
}
