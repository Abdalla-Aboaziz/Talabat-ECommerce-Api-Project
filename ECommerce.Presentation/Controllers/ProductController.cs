using ECommerce.Presentation.Attributes;
using ECommerce.ServiceAbstraction;
using ECommerce.Shared;
using ECommerce.Shared.ProductDtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.Presentation.Controllers
{
    
    public class ProductController : ApiBaseController
    {
        private readonly IProductServices _productServices;

        public ProductController(IProductServices productServices)
        {
            _productServices = productServices;
        }
        // Get All Products

        [HttpGet]
        [RedisCashe]
       
        // GET: api/Product
        public async Task<ActionResult<IEnumerable<ProductDTO>>> GetAllProducts([FromQuery]ProductQueryParams queryParams)
        {
            var products = await _productServices.GetAllProductsAsync(queryParams);
            return Ok(products);
        }

        // Get Product By Id

        [HttpGet("{id}")]
        public async Task<ActionResult<ProductDTO>> GetProductById(int id)
        {
          
            var result = await _productServices.GetProductByIdAsync(id);

            return HandelResult<ProductDTO>(result);
           
        }
        // Get All Brands
        [HttpGet("brands")]
        public async Task<ActionResult<IEnumerable<ProductBrandDTO>>> GetAllBrands()
        {
            var brands = await _productServices.GetAllBrandsAsync();
            return Ok(brands);
        }
        // Get All Types
        [HttpGet("types")]
        public async Task<ActionResult<IEnumerable<ProductTypeDTO>>> GetAllTypes()
        {
            var types = await _productServices.GetAllTypesAsync();
            return Ok(types);
        }

    }
}
