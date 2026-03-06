using ECommerce.Domain.Contracts;
using ECommerce.Domain.Entities.BasketModules;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace ECommerce.Presistance.Repository
{
    public class BasketRepository : IBasketRepository
    {
      
        private readonly IDatabase _database;


        public BasketRepository(IConnectionMultiplexer connection)
        {
            _database = connection.GetDatabase();
        }
        public async Task<CustomerBasket?> CreateOrUpdateBasketAsync(CustomerBasket basket, TimeSpan timetolive = default)
        {
            var JsonBasket =JsonSerializer.Serialize(basket);
            var IsCreatedOrUpdated = await  _database.StringSetAsync(basket.Id, JsonBasket,
                (timetolive==default)?TimeSpan.FromDays(7):timetolive);
            if (IsCreatedOrUpdated)
            {
                return await GetBasketAsync(basket.Id);
            }
            else
            { 
                return null;
            }
        }

        public async Task<bool> DeleteCustomerBasketAsync(string basketId)=> await _database.KeyDeleteAsync(basketId);


        public async Task<CustomerBasket?> GetBasketAsync(string basketId)
        {
           var Basket= await _database.StringGetAsync(basketId);
            if (Basket.IsNullOrEmpty)
            {
                return null;
            }
            else
            {
                return JsonSerializer.Deserialize<CustomerBasket>(Basket!);
            }
        }
    }
}
