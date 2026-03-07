using ECommerce.Domain.Entities.OrderModules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Service.Specifications
{
    public class OrderWithPaymentIntentSpecification:BaseSpecification<Order,Guid>
    {
        public OrderWithPaymentIntentSpecification(string paymentId):base(o=>o.PaymentIntendId == paymentId)
        {
            
        }
    }
}
