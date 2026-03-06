using ECommerce.Domain.Entities.OrderModules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Service.Specifications
{
    public class OrderSpecifications:BaseSpecification<Order,Guid>
    {
        public OrderSpecifications(Guid id ,string userEmail):base(o=>o.Id==id && o.UserEmail.ToLower()==userEmail.ToLower())
        {
            IncludeExpression.Add(o => o.Items);
            IncludeExpression.Add(o => o.DeliveryMethod);
        }
        public OrderSpecifications(string userEmail):base(o=>o.UserEmail.ToLower() == userEmail.ToLower())
        {
            IncludeExpression.Add(o => o.Items);
            IncludeExpression.Add(o => o.DeliveryMethod);
            AddOrderByDescending(o => o.OrderDate);
        }
    }
}
