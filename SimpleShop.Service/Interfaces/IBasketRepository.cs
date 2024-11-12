using SimpleShop.Core.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleShop.Service.Interfaces
{
    public interface IBasketRepository
    {
        Task AddBasket(Basket basket);
        Task<Basket> GetBasketByUser(ApplicationUser user, bool trackChanges);
        Task ClearBasket(ApplicationUser user, bool trackChanges);
        Task DeleteBasket(Basket basket);
        Task UpdateBasket(Basket basket);
    }
}
