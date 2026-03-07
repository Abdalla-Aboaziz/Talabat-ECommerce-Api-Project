using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Shared.IdentityDtos
{
    public class IdentityAddressDto
    {
       
            public string City { get; set; } = default!;
            public string Street { get; set; } = default!;

            public string Country { get; set; } = default!;

            public string FistName { get; set; } = default!;
            public string LastName { get; set; } = default!;
        

    }
}
