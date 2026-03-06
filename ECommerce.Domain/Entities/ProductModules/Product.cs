

namespace ECommerce.Domain.Entities.ProductModules
{
    public  class Product:BaseEnitiy<int>
    {
       
        public string Name { get; set; } = default!;
        public string Description { get; set; } = default!;
        public string PictureUrl { get; set; } = default!;
        public decimal Price { get; set; }

        #region RelationShip
        public int BrandId { get; set; }
        public ProductBrand ProductBrand { get; set; } = default!;

        public int TypeId { get; set; }
        public ProductType ProductType { get; set; } = default!;

        #endregion
        // test initial commit

    }
}
