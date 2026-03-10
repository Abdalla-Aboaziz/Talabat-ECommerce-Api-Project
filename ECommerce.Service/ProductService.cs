using AutoMapper;
using ECommerce.Domain.Contracts;
using ECommerce.Domain.Entities.ProductModules;
using ECommerce.Service.Exceptions;
using ECommerce.Service.Specifications;
using ECommerce.ServiceAbstraction;
using ECommerce.Shared;
using ECommerce.Shared.CommonResult;
using ECommerce.Shared.ProductDtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Service
{
    public class ProductService : IProductServices
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public ProductService(IUnitOfWork unitOfWork,IMapper mapper)
        {
          _unitOfWork = unitOfWork;
           _mapper = mapper;
        }
        public async Task<IEnumerable<ProductBrandDTO>> GetAllBrandsAsync()
        {
            var brands = await _unitOfWork.GetRepository<ProductBrand,int>().GetAllAsync();
            // mapping from ProductBrand to ProductBrandDTO
            return _mapper.Map<IEnumerable<ProductBrandDTO>>(brands);

        }

        public async Task<PaginatedResult<ProductDTO>> GetAllProductsAsync(ProductQueryParams queryParams)
        {
            var spec = new ProductWithBrandsAndTypeSpecification(queryParams);
            var products =await  _unitOfWork.GetRepository<Product,int>().GetAllAsync(spec);
            // mapping from Product to ProductDTO
            var DataToReturn= _mapper.Map<IEnumerable<ProductDTO>>(products);
            var CountOfReturnedData = DataToReturn.Count(); //
            var CountSpec = new ProductCountSpecifications(queryParams);
            var CountOfAllData = await _unitOfWork.GetRepository<Product,int>().CountAsync(CountSpec);
            return new PaginatedResult<ProductDTO>(queryParams.PageIndex, queryParams.PageSize, CountOfAllData, DataToReturn);
          
        }

        public async Task<IEnumerable<ProductTypeDTO>> GetAllTypesAsync()
        {
           var types = await _unitOfWork.GetRepository<ProductType,int>().GetAllAsync();
            // mapping from ProductType to ProductTypeDTO
            return _mapper.Map<IEnumerable<ProductTypeDTO>>(types);
        }

        public async Task<Result<ProductDTO>> GetProductByIdAsync(int id)
        {
            var spec = new ProductWithBrandsAndTypeSpecification(id);

            var product = await _unitOfWork.GetRepository<Product,int>().GetByIdAsync(spec);
            if (product is null )
            {
               // throw new ProductNotFoundException(id);
              return  Error.NotFound("Proudct Not Found",$"Proudct with Id : {id} Not Found");

            }
            // mapping from Product to ProductDTO
             var map = _mapper.Map<ProductDTO>(product);
            return(map);
        }
    }
}
