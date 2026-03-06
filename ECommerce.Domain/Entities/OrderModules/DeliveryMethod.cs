

namespace ECommerce.Domain.Entities.OrderModules
{
    public class DeliveryMethod: BaseEnitiy<int>
    {
        public string ShortName { get; set; } = default!;
        public string Description { get; set; } = default!;
        public string DeliveryTime { get; set; } = default!;
        public decimal Price { get; set; }

    }
}
