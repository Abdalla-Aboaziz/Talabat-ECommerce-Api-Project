using AutoMapper;
using ECommerce.Domain.Contracts;
using ECommerce.Domain.Entities.OrderModules;
using ECommerce.Domain.Entities.ProductModules;
using ECommerce.ServiceAbstraction;
using ECommerce.Shared.BasketDtos;
using ECommerce.Shared.CommonResult;
using Microsoft.Extensions.Configuration;
using Stripe;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Product = ECommerce.Domain.Entities.ProductModules.Product;

namespace ECommerce.Service
{
    public class PaymentService : IPaymentService
    {
        private readonly IBasketRepository _basketRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IConfiguration _configuration;
        private readonly IMapper _mapper;

        public PaymentService(IBasketRepository basketRepository,IUnitOfWork unitOfWork,IConfiguration configuration,IMapper mapper)
        {
            _basketRepository = basketRepository;
           _unitOfWork = unitOfWork;
           _configuration = configuration;
         _mapper = mapper;
        }
        public async Task<Result<BasketDtos>> CreatePaymentIntentAsync(string BasketId)
        {
            // Calculate Amount = Subtota+ DeliveryMethod Cost

            // Get Basket By Id
            var Basket =await _basketRepository.GetBasketAsync(BasketId);
            if (Basket == null) return Error.NotFound();
            //Check Proudct And Its Price 
            foreach (var item in Basket.Items)
            {
                var Product = await _unitOfWork.GetRepository<Product, int>().GetByIdAsync(item.Id);
                if (Product == null) return Error.NotFound();
                item.Price= Product.Price;

            }
            //Calculate SubTotal
            var subTotal = Basket.Items.Sum(i => i.Price * i.Quantity);
            //Get Delivery Mehod By Id
            if(!Basket.DeliveryMethodId.HasValue) return Error.NotFound();
            var delivery = await _unitOfWork.GetRepository<DeliveryMethod, int>().GetByIdAsync(Basket.DeliveryMethodId.Value);
            Basket.ShippingCost = delivery!.Price;

            var Amount = subTotal + delivery.Price;
            //Send Amount To Stripe
            StripeConfiguration.ApiKey = _configuration["StripeOption:SecretKey"];

            PaymentIntentService paymentIntentService = new PaymentIntentService();
            PaymentIntent paymentIntent;

            if(Basket.PaymentIntentId is null)
            {
                // Create
                var Option = new PaymentIntentCreateOptions()
                {
                    Amount = (long)Amount * 100,
                    Currency = "usd",
                    PaymentMethodTypes = new List<string>() { "card" }
                };
                paymentIntent=await paymentIntentService.CreateAsync(Option);
            }
            else
            {
                var options = new PaymentIntentUpdateOptions()
                {
                    Amount = (long)Amount * 100,
                };
                paymentIntent = await paymentIntentService.UpdateAsync(Basket.PaymentIntentId, options);
            }
            Basket.PaymentIntentId=paymentIntent.Id;
            Basket.ClientSecret=paymentIntent.ClientSecret;

            Basket = await _basketRepository.CreateOrUpdateBasketAsync(Basket);
            return _mapper.Map<BasketDtos>(Basket);
        }
    }
}
