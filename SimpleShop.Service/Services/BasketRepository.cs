using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using SimpleShop.Core.Model;
using SimpleShop.Repo.Data;
using SimpleShop.Repo.GenericRepository.Service;
using SimpleShop.Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace SimpleShop.Service.Services
{
    public class BasketRepository: RepositoryBase<Basket>, IBasketRepository
    {
        public BasketRepository(RepositoryContext repositoryContext) : base(repositoryContext)
        {
        }
        public async Task<Basket> GetBasketByUser(ApplicationUser user, bool trackChanges) {

            var basket = await FindByConditionAsyncWithIncludeCollection(
                b => b.User.Id == user.Id,
                b => b.BasketProducts,
                bp => bp.Product
            ).Result.FirstOrDefaultAsync();
            if (basket != null)
                return basket;
            basket = new Basket() { BasketProducts = new List<BasketProduct>(), User = user };
            var x = await CreateAsync(basket);
            return x.Entity; 
        }

        public async Task AddBasket(Basket basket) =>
            await CreateAsync(basket);
        public async Task DeleteBasket(Basket basket) =>
            await RemoveAsync(basket);
        public async Task UpdateBasket(Basket basket) =>
            await UpdateAsync(basket);
    }
}
