

namespace ECommerce.Domain.Contracts
{
    public interface IGenericRepository<TEntity,TKey> where TEntity : BaseEnitiy<TKey>
    {
        Task<IEnumerable<TEntity>> GetAllAsync(ISpecification<TEntity,TKey> specification);
        Task<IEnumerable<TEntity>> GetAllAsync();

        Task<TEntity?> GetByIdAsync(TKey id);
        Task<TEntity?> GetByIdAsync(ISpecification<TEntity,TKey> specification);
        Task AddAsync(TEntity entity);
        void Update(TEntity entity);
        void Remove(TEntity entity);
        Task<int> CountAsync(ISpecification<TEntity,TKey> specification);
      

    }
}
