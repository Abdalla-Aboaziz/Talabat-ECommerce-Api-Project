

namespace ECommerce.Domain.Entities.OrderModules
{
    public class Order : BaseEnitiy<Guid>
    {
        public string UserEmail { get; set; } = default!;
        public DateTimeOffset OrderDate { get; set; } = DateTimeOffset.Now;
        public OrderStatus OrderStatus { get; set; } = OrderStatus.pending;
        public DeliveryMethod DeliveryMethod { get; set; } = default!; //Navigational property 1:m
        public OrderAddress Address { get; set; } = default!;
        public int DeliveryMethodId { get; set; } //fk
        public ICollection<OrderItem> Items { get; set; } =[];
        public decimal Subtotal { get; set; }
        public decimal getTotal() => Subtotal + DeliveryMethod.Price;

        public string? PaymentIntendId { get; set; }

    }
}
