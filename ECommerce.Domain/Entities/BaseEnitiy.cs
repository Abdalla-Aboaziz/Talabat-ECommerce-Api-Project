
namespace ECommerce.Domain.Entities
{
    public abstract class BaseEnitiy<TKey>
    {
        public TKey Id { get; set; } = default!;
    }
}
