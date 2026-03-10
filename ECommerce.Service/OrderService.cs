using AutoMapper;
using ECommerce.Domain.Contracts;
using ECommerce.Domain.Entities.BasketModules;
using ECommerce.Domain.Entities.OrderModules;
using ECommerce.Domain.Entities.ProductModules;
using ECommerce.Service.Specifications;
using ECommerce.ServiceAbstraction;
using ECommerce.Shared.CommonResult;
using ECommerce.Shared.OrderDtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Service
{
    public class OrderService : IOrderService
    {
        private readonly IMapper _mapper;
        private readonly IBasketRepository _basketRepository;
        private readonly IUnitOfWork _unitOfWork;

        public OrderService(IMapper mapper,IBasketRepository basketRepository,IUnitOfWork unitOfWork)
        {
           _mapper = mapper;
           _basketRepository = basketRepository;
            _unitOfWork = unitOfWork;
        }
        public async Task<Result<OrderToReturnDto>> CreateOrderAsync(OrderDto orderDto, string Email)
        {
            // Map Addres 
            // 3. Get OrderAddress
            var OrderAddres = _mapper.Map<OrderAddress>(orderDto.ShipToAddress);
            // 5.1 Get Basket From Basket Repository=> get BasketId for OrderItems
            var Basket = await _basketRepository.GetBasketAsync(orderDto.BasketId);
            if (Basket is null) return Error.NotFound("Basket not found",$"Basket with id {orderDto.BasketId} Not Found ");
            // 5. Get OrderItems
                List<OrderItem> orderItems = new List<OrderItem>();
                // 5.2Convert Every Basket Item To OrderItem 
                foreach (var item in Basket.Items)
                {
                    var Proudct = await _unitOfWork.GetRepository<Product,int>().GetByIdAsync(item.Id);
                    if (Proudct is null) return Error.NotFound("Product not found", $"Product with id {item.Id} Not Found ");
                 
                    orderItems.Add(CreateOrderItem(item, Proudct));

                }
            // 4. Get Delivery Method
            var DeliveryMethod = await _unitOfWork.GetRepository<DeliveryMethod, int>().GetByIdAsync(orderDto.DeliveryMethodId);
            if ( DeliveryMethod is null ) return Error.NotFound("Delivery method not found", $"Delivery method with id {orderDto.DeliveryMethodId} Not Found ");
            // 6. Calculate Subtotal
            var subtotal = orderItems.Sum(item => item.Price * item.Quantity);
            // 2. Create Order Object
            // 7. TODO :: PaymentIntendId
            //Check order Exists
            var spec = new OrderWithPaymentIntentSpecification(Basket.PaymentIntentId);
            var existorder = await _unitOfWork.GetRepository<Order, Guid>().GetByIdAsync(spec);
            if (existorder is not null)
            {
                _unitOfWork.GetRepository<Order, Guid>().Remove(existorder);
                await _unitOfWork.SaveChangesAsync(); 
            }

            var order = new Order
            {
                Address = OrderAddres,
                DeliveryMethod = DeliveryMethod,
                Items = orderItems,
                Subtotal = subtotal,
                UserEmail = Email,
                PaymentIntendId = Basket.PaymentIntentId
            };

            await _unitOfWork.GetRepository<Order, Guid>().AddAsync(order);
            int result = await _unitOfWork.SaveChangesAsync();
            if (result == 0) return Error.Failure("Failed to create order", "An error occurred while creating the order.");
            return _mapper.Map<OrderToReturnDto>(order);

        }

        public async Task<IEnumerable<DeliveryMethodDto>> GetAllDeliveryMethodsAsync()
        {
           var Delivery=await _unitOfWork.GetRepository<DeliveryMethod,int>().GetAllAsync();
            return _mapper.Map<IEnumerable<DeliveryMethodDto>>(Delivery);
        }

        public async Task<Result<OrderToReturnDto>> GetOrderByIdAsync(Guid id, string Email)
        {
            var spec = new OrderSpecifications(id, Email);
             var order= await _unitOfWork.GetRepository<Order,Guid>().GetByIdAsync(spec);
            if (order is null) return Error.NotFound("Order not found", $"Order with id {id} Not Found ");
            return _mapper.Map<OrderToReturnDto>(order);

        }

        public async Task<IEnumerable<OrderToReturnDto>> GetOrdersForUserAsync(string Email)
        {
            var spec = new OrderSpecifications(Email);

            var orders = await _unitOfWork
                .GetRepository<Order, Guid>()
                .GetAllAsync(spec);

            var mappedOrders = _mapper.Map<IEnumerable<OrderToReturnDto>>(orders);

            return mappedOrders; 
        }

        #region Helper Method
        private static OrderItem CreateOrderItem(BasketItem item, Product product)
        {
            return new OrderItem
            {
                Product = new ProductItemOrder
                {
                    ProductId = product.Id,
                    ProductName = product.Name,
                    PictureUrl = product.PictureUrl
                },
                Price = product.Price,
                Quantity = item.Quantity
            };
        }

        #endregion
    }
}
