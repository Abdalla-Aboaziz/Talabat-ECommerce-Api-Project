using ECommerce.Shared;
using ECommerce.Shared.CommonResult;
using ECommerce.Shared.ProductDtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.ServiceAbstraction
{
    public interface IProductServices
    {
        // Get all products
        Task<PaginatedResult<ProductDTO>> GetAllProductsAsync(ProductQueryParams queryParams);
        // Get product by ID
        Task<Result<ProductDTO>> GetProductByIdAsync(int id);
        // Get All Brands 
        Task<IEnumerable<ProductBrandDTO>> GetAllBrandsAsync();
        // Get All Types
        Task<IEnumerable<ProductTypeDTO>> GetAllTypesAsync();

    }
}
