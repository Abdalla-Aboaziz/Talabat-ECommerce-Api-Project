using AutoMapper;
using ECommerce.Domain.Entities.OrderModules;
using ECommerce.Shared.OrderDtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Service.MappingProfiles
{
    public class OrderProfile:Profile
    {
        public OrderProfile()
        {
            CreateMap<AddressDto, OrderAddress>().ReverseMap();

            CreateMap<OrderItem, OrderItemDto>()
                .ForMember(d => d.ProductName, o => o.MapFrom(s => s.Product.ProductName))
                .ForMember(d => d.PictureUrl, o => o.MapFrom<OrderItemPictureUrlResolver>())
                .ForMember(d => d.Quantity, o => o.MapFrom(s => s.Quantity));

            CreateMap<Order, OrderToReturnDto>()
                .ForMember(d => d.DeliveryMethod, o => o.MapFrom(s => s.DeliveryMethod.ShortName))
                .ForMember(d => d.OrderStatus, o => o.MapFrom(s => s.OrderStatus.ToString()))
                .ForMember(d => d.Total, o => o.MapFrom(s => s.getTotal()));

            CreateMap<DeliveryMethod, DeliveryMethodDto>();
        }
    }
}
