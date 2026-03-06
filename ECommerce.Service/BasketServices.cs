using AutoMapper;
using ECommerce.Domain.Contracts;
using ECommerce.Domain.Entities.BasketModules;
using ECommerce.Service.Exceptions;
using ECommerce.ServiceAbstraction;
using ECommerce.Shared.BasketDtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Service
{
    public class BasketServices : IBasketServices
    {
        private readonly IBasketRepository _basketRepository;
        private readonly IMapper _mapper;

        public BasketServices(IBasketRepository basketRepository ,IMapper mapper)
        {
           _basketRepository = basketRepository;
          _mapper = mapper;
        }
        public async Task<BasketDtos> CreateOrUpdateBasketAsync(BasketDtos basket)
        {
            var CustomeBasket =_mapper.Map<BasketDtos,CustomerBasket>(basket);

            var CreatedOrUpdatedBasket = await _basketRepository.CreateOrUpdateBasketAsync(CustomeBasket);

            return _mapper.Map<CustomerBasket,BasketDtos>(CreatedOrUpdatedBasket!);

        }

        public async Task<bool> DeleteBasketAsync(string Id)=> await _basketRepository.DeleteCustomerBasketAsync(Id);


        public async Task<BasketDtos> GetBasketAsync(string Id)
        {
           var Basket= await _basketRepository.GetBasketAsync(Id);

           if (Basket is null) throw new BasketNotFoundException(Id);

            return _mapper.Map<CustomerBasket,BasketDtos>(Basket);
        }
    }
}
