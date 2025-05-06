using SimpleShop.Core.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleShop.Service.Interfaces
{
    public interface IBasketProductRepository
    {
        Task DeleteBasketProduct(BasketProduct basketProduct);
        Task UpdateBasketProduct(BasketProduct basketProduct);
        Task ClearBasketProducts(int basketId);
    }
}
