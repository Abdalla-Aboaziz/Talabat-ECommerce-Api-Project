

namespace ECommerce.Domain.Entities.BasketModules
{
    public class CustomerBasket
    {
        public string Id { get; set; } =default!;
        public ICollection<BasketItem> Items { get; set; } = new List<BasketItem>();
        // payment
        public int? DeliveryMethodId { get; set; }
        public string? PaymentIntentId { get; set; }
        public string? ClientSecret { get; set; }
        public decimal ? ShippingCost { get; set; }


    }
}
