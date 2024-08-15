using Microsoft.EntityFrameworkCore;
using SimpleShop.Core.Model;
using SimpleShop.Repo.Data;
using SimpleShop.Repo.GenericRepository.Intarface;
using SimpleShop.Repo.GenericRepository.Service;
using SimpleShop.Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleShop.Service.Services
{
    public class BasketManager : IBasketManager
    {
        
        private IRepositoryManager _repositoryManager;
        public BasketManager(IRepositoryManager repositoryManager) 
        {
            _repositoryManager = repositoryManager;
        }

        public async Task AddProductToBasket(ApplicationUser user, string productArticle, int count)
        {
            var basket = await GetBasket(user);
            var basketProduct = basket.BasketProducts.FirstOrDefault(b=>b.Product.Article == productArticle);
            var product = await _repositoryManager.ProductRepository.GetProduct(productArticle);
            if (basketProduct == null)
            {
                basketProduct = new BasketProduct()
                {
                    ProductId = product.Id,
                    Product = product,
                    Basket = basket,
                    Count = count
                };
                basket.BasketProducts.Add(basketProduct);
            }
            else
            {
                basketProduct.Count += count;
            }
            await _repositoryManager.BasketRepository.UpdateBasket(basket);
            await _repositoryManager.SaveAsync();
        }

        public async Task<Basket> GetBasket(ApplicationUser user)
        {
            var basket = await _repositoryManager.BasketRepository.GetBasketByUser(user, false);
            await _repositoryManager.SaveAsync();
            return basket;
        }

        public async Task RemoveProductFromBasket(ApplicationUser user, string productArticle)
        {
            var basket = await GetBasket(user);
            var basketProduct = basket.BasketProducts.FirstOrDefault(basket => basket.Product.Article == productArticle);
            if (basketProduct != null)
            {
                await _repositoryManager.BasketProductRepository.DeleteBasketProduct(basketProduct);
                await _repositoryManager.SaveAsync();
            }
        }
        public async Task ChangeProductCount(ApplicationUser user, string productArticle, int count)
        {
            var basket = await GetBasket(user);
            var basketProduct = basket.BasketProducts.FirstOrDefault(basket => basket.Product.Article == productArticle);
            if (basketProduct != null)
            {
                basketProduct.Count += count;
                await _repositoryManager.BasketProductRepository.UpdateBasketProduct(basketProduct);
                await _repositoryManager.SaveAsync();
            }
        }
    }
}
