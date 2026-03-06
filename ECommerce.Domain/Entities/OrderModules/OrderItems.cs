

namespace ECommerce.Domain.Entities.OrderModules
{
    public class OrderItem : BaseEnitiy<int>
    {
        public ProductItemOrder Product { get; set; } = default!;
        public decimal Price { get; set; }
        public int Quantity { get; set; }
    }
}
