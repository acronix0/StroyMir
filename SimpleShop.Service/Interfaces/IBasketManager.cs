using SimpleShop.Core.Model;
using SimpleShop.Repo.GenericRepository.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleShop.Service.Interfaces
{
    public interface IBasketManager
    {
        Task<Basket> GetBasket(ApplicationUser user);
        Task<string> AddProductToBasket(ApplicationUser user, string productArticle, int count);
        Task RemoveProductFromBasket(ApplicationUser user, string productArticle);
        Task<string> ChangeProductCount(ApplicationUser user, string productArticle, int count);

    }
}
