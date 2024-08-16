using SimpleShop.Service.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleShop.Service.Interfaces
{
    public interface IRepositoryManager
    {
     
        ICategoryRepository CategoryRepository { get; }
        IProductRepository ProductRepository { get; }
        IBasketRepository BasketRepository { get; }
        IBasketProductRepository BasketProductRepository { get; }
        IOrderRepository OrderRepository { get; }
        IUserAuthenticationRepository UserAuthentication {  get; }
        Task SaveAsync();
    }
}
