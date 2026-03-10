using ECommerce.Domain.Entities.ProductModules;
using ECommerce.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Service.Specifications
{
    public class ProductCountSpecifications:BaseSpecification<Product,int>
    {
        public ProductCountSpecifications(ProductQueryParams queryParams):base(p => (!queryParams.BrandId.HasValue || p.BrandId == queryParams.BrandId.Value) && (queryParams.TypeId == null || p.TypeId == queryParams.TypeId) &&
            (string.IsNullOrEmpty(queryParams.Search) || p.Name.ToLower().Contains(queryParams.Search.ToLower())))
        {
            
        }
    }
}
