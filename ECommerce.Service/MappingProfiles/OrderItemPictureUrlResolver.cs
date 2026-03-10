using AutoMapper;
using ECommerce.Domain.Entities.OrderModules;
using ECommerce.Shared.OrderDtos;
using Microsoft.Extensions.Configuration;

namespace ECommerce.Service.MappingProfiles
{
    public class OrderItemPictureUrlResolver : IValueResolver<OrderItem, OrderItemDto, string>
    {
        private readonly IConfiguration _configuration;

        public OrderItemPictureUrlResolver(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public string Resolve(OrderItem source, OrderItemDto destination, string destMember, ResolutionContext context)
        {
            if (string.IsNullOrEmpty(source.Product.PictureUrl)) return string.Empty;

            if (source.Product.PictureUrl.StartsWith("http"))
            {
                return source.Product.PictureUrl;
            }
            var BaseUrl = _configuration.GetSection("URLs")["BaseURL"];
            if (string.IsNullOrEmpty(BaseUrl)) return string.Empty;
            return $"{BaseUrl}/{source.Product.PictureUrl}";

        }

    }
}
