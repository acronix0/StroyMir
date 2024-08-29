using Microsoft.EntityFrameworkCore;
using SimpleShop.Core.Model;
using SimpleShop.Repo.Data;
using SimpleShop.Repo.GenericRepository.Service;
using SimpleShop.Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleShop.Service.Services
{
    public class OrderRepository : RepositoryBase<Order>, IOrderRepository
    {
        public OrderRepository(RepositoryContext repositoryContext) : base(repositoryContext)
        {
        }

        public async Task AddOrder(Order order)
        {
           
           await CreateAsync(order);
        }

        public async Task DeleteOrder(Order order)
        {
            await RemoveAsync(order);
        }

        public async Task<IEnumerable<Order>> GetUserOrders(string userId)
        {
            var query = await FindByConditionAsyncWithIncludeCollection(
                o => o.User.Id == userId,
                op => op.OrderProducts,
                op => op.Product).Result.ToListAsync() ;
            return  query;
        }

        public Task<IEnumerable<Order>> GetOrders()
        {
            throw new NotImplementedException();
        }

        public Task UpdateOrder(Order order)
        {
            throw new NotImplementedException();
        }

        public async Task<Order> GetOrder(int id)
        {
            var query = await FindByConditionAsyncWithIncludeCollection(
              o => o.Id == id,
              op => op.OrderProducts,
              op => op.Product).Result.FirstOrDefaultAsync();
            return query;
        }
    }
}
