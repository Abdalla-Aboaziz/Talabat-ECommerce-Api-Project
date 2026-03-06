

namespace ECommerce.Domain.Contracts
{
    public interface ISpecification<TEntity,TKey> where TEntity : BaseEnitiy<TKey>
    {
        public ICollection<Expression<Func<TEntity,object>>> IncludeExpression { get; }
        public Expression<Func<TEntity,bool>>Criteria { get; }
        public Expression<Func<TEntity,object>>? OrderBy { get; }
        public Expression<Func<TEntity,object>>? OrderByDescending { get; }

        public int Take { get; }
        public int Skip { get; }
        public bool IsPaginated { get; }

    }
}
