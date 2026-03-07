using AutoMapper;
using ECommerce.Domain.Entities.IdentityModule;
using ECommerce.Shared.IdentityDtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Service.MappingProfiles
{
    public  class AuthProfile:Profile
    {
        public AuthProfile()
        {
            CreateMap<Address,IdentityAddressDto>().ReverseMap();
        }
    }
}
