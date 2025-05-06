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
    public class BasketProductRepository : RepositoryBase<BasketProduct>,IBasketProductRepository
    {
        public BasketProductRepository(RepositoryContext repositoryContext) : base(repositoryContext)
        {
        }

        public async Task DeleteBasketProduct(BasketProduct basketProduct) =>
            await RemoveAsync(basketProduct);
        public async Task UpdateBasketProduct(BasketProduct basketProduct) =>
            await UpdateAsync(basketProduct);
        public async Task ClearBasketProducts(int basketId) =>
           await DeleteWhereAsync<BasketProduct>(p => p.BasketId == basketId);
    }
}
