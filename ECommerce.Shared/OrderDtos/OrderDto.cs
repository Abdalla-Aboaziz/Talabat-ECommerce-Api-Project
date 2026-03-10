using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Shared.OrderDtos
{
   public record OrderDto(string BasketId,int DeliveryMethodId,AddressDto ShipToAddress);
}
