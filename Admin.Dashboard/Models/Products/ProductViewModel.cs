using ECommerce.Domain.Entities.ProductModules;
using System.ComponentModel.DataAnnotations;

namespace Admin.Dashboard.Models.Products
{
    public class ProductViewModel
    {
        public int Id { get; set; }
        [Required(ErrorMessage ="Product Name is Required.")]
        public string Name { get; set; }
        [Required(ErrorMessage = "Product Description is Required.")]
        public string Description { get; set; }
        public IFormFile Image { get; set; }

        public string? PictureUrl { get; set; }
        [Required(ErrorMessage = "Product Price is Required.")]
        [Range(1,int.MaxValue)]
        public decimal Price { get; set; }
        [Required(ErrorMessage = "Product BrandId is Required.")]
        public  int BrandId { get; set; }   
        public ProductBrand? Brand { get; set; }
        [Required(ErrorMessage = "Product TypeId is Required.")]
        public int TypeId { get; set; }
        public ProductType? Type { get; set; }

    }
}
