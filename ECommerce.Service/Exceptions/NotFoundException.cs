 using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Service.Exceptions
{
    public abstract class NotFoundException(string Message):Exception(Message) 
    {
        
    }

    public sealed class ProductNotFoundException(int id)
        : NotFoundException($"Product with Id {id} was not found.");
  

    public sealed class BasketNotFoundException(string id): NotFoundException($"Basket With Id : {id} Not Found");
}
