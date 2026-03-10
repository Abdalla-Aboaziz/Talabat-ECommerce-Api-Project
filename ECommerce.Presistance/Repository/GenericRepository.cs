using ECommerce.Domain.Contracts;
using ECommerce.Domain.Entities;
using ECommerce.Presistance.Data.DBContexts;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Presistance.Repository
{
    public class GenericRepository<TEnity, TKey> : IGenericRepository<TEnity, TKey> where TEnity : BaseEnitiy<TKey>
    {
        private readonly StoreDbContext _dbContext;

        public GenericRepository(StoreDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task AddAsync(TEnity entity)=> await _dbContext.Set<TEnity>().AddAsync(entity);

        public async Task<int> CountAsync(ISpecification<TEnity, TKey> specification)
        {
           return await SpecificationEvaluator.CreateQuery(_dbContext.Set<TEnity>(), specification).CountAsync(); // we are using CountAsync to get count of entities based on specification
        }

        public async Task<IEnumerable<TEnity>> GetAllAsync() =>await _dbContext.Set<TEnity>().ToListAsync();

        public async Task<IEnumerable<TEnity>> GetAllAsync(ISpecification<TEnity, TKey> specification)
        {
           var Query = SpecificationEvaluator.CreateQuery(_dbContext.Set<TEnity>(), specification); 

              return await Query.ToListAsync(); // we are using ToListAsync to get all entities based on specification
        }

        public async Task<TEnity?> GetByIdAsync(TKey id) => await _dbContext.Set<TEnity>().FindAsync(id);

        public async Task<TEnity?> GetByIdAsync(ISpecification<TEnity, TKey> specification)
        {
           var Query = await SpecificationEvaluator.CreateQuery(_dbContext.Set<TEnity>(), specification)
                .FirstOrDefaultAsync(); // we are using FirstOrDefaultAsync to get single entity based on specification
            return Query;
        }

        public void Remove(TEnity entity) => _dbContext.Set<TEnity>().Remove(entity);


        public void Update(TEnity entity) => _dbContext.Set<TEnity>().Update(entity);

    }
}
