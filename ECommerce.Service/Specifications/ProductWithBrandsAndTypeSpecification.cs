using ECommerce.Domain.Entities.ProductModules;
using ECommerce.Shared;

namespace ECommerce.Service.Specifications
{
    public class ProductWithBrandsAndTypeSpecification : BaseSpecification<Product, int>
    {
        //This specification is used to include the ProductBrand and ProductType navigation properties

        // (Get Product By Id)
        public ProductWithBrandsAndTypeSpecification(int id) : base(p => p.Id == id)
        {
            AddInclude(p => p.ProductBrand);
            AddInclude(p => p.ProductType);

        }
        //  (Get All Products)
        public ProductWithBrandsAndTypeSpecification(ProductQueryParams queryParams,bool forDashboard =false)
            : base(p => (!queryParams.BrandId.HasValue || p.BrandId == queryParams.BrandId.Value) && (queryParams.TypeId == null || p.TypeId == queryParams.TypeId) &&
            (string.IsNullOrEmpty(queryParams.Search) || p.Name.ToLower().Contains(queryParams.Search.ToLower())))
        {
            //brandid is not null
            // typeid is not null
            // typeid and brandid is not null
            AddInclude(p => p.ProductBrand);
            AddInclude(p => p.ProductType);

            // Sorting
            switch (queryParams.SortingOptions)
            {
                case ProductSortingOptions.NameAsc:
                    AddOrderBy(p => p.Name);
                    break;
                case ProductSortingOptions.NameDesc:
                    AddOrderByDescending(p => p.Name);
                    break;
                case ProductSortingOptions.PriceAsc:
                    AddOrderBy(p => p.Price);
                    break;
                case ProductSortingOptions.PriceDesc:
                    AddOrderByDescending(p => p.Price);
                    break;
                default:
                    AddOrderBy(p => p.Id);
                    break;
            }
            // Pagination
            if (!forDashboard)
                ApplyPagination(queryParams.PageSize, queryParams.PageIndex);
        }
    }
}
