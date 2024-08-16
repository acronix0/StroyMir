using SimpleShop.Core.Model;
using SimpleShop.Repo.GenericRepository.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleShop.Service.Interfaces
{
    public interface IOrderRepository 
    {
        Task<IEnumerable<Order>> GetOrders();
        Task AddOrder(Order order);
        Task DeleteOrder(Order order);
        Task<Order> GetOrder(int id);
        Task<IEnumerable<Order>> GetUserOrders(string userId);
        Task UpdateOrder(Order order);

    }
}
