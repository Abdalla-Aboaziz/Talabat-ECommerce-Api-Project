

namespace ECommerce.Domain.Entities.OrderModules
{
    // part of the orderitem Tabel, we need to save the product details at the time of order
    public class ProductItemOrder
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; } = default!;
        public string PictureUrl { get; set; } = default!;
    }
}
