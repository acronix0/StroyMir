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

        public async Task<string> AddProductToBasket(ApplicationUser user, string productArticle, int count)
        {
            try
            {
                var basket = await GetBasket(user);
                if (basket == null)
                    return "Не удалось получить корзину пользователя.";

                int inStock = -1;
                var product = await _repositoryManager.ProductRepository.GetProduct(productArticle);
                if (product == null)
                    return "Продукт с указанной статьей не найден.";

                inStock = product.Count;

                var basketProduct = basket.BasketProducts.FirstOrDefault(b => b.Product.Article == productArticle);
                int totalRequested = count;

                if (basketProduct != null)
                    totalRequested += basketProduct.Count;

                if (totalRequested > inStock)
                    return $"Недостаточно товара на складе. В наличии: {inStock}, требуется: {totalRequested}.";

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

                return "";
            }
            catch (Exception)
            {
                return "Произошла ошибка при добавлении товара в корзину.";
            }
        }

        public async Task<Basket> GetBasket(ApplicationUser user)
        {
            var basket = await _repositoryManager.BasketRepository.GetBasketByUser(user, false);
            await _repositoryManager.SaveAsync();
            return basket;
        }
        public async Task ClearBasket(ApplicationUser user)
        {
            var basket = await GetBasket(user);
            await _repositoryManager.BasketProductRepository.ClearBasketProducts(basket.Id);
            await _repositoryManager.SaveAsync();
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
        public async Task<string> ChangeProductCount(ApplicationUser user, string productArticle, int count)
        {
            var basket = await GetBasket(user);
            int inStock = -1;
            var product = await _repositoryManager.ProductRepository.GetProduct(productArticle);
            if (product != null)
            {
                inStock = product.Count;
            }
            var basketProduct = basket.BasketProducts.FirstOrDefault(basket => basket.Product.Article == productArticle);

            if (basketProduct == null)
            {
                if (inStock < basketProduct.Count)
                {
                    return "Недостаточно товара на складе, в наличии: " + inStock + " шт.";
                }
                basketProduct = new BasketProduct()
                {
                    ProductId = product.Id,
                    Product = product,
                    Basket = basket,
                    Count = count
                };
                basket.BasketProducts.Add(basketProduct);
            }
            if (basketProduct != null)
            {
                if (inStock < basketProduct.Count)
                {
                    return "Недостаточно товара на складе, в наличии: " + inStock + " шт.";
                }
                basketProduct.Count = count;
                await _repositoryManager.BasketProductRepository.UpdateBasketProduct(basketProduct);
                await _repositoryManager.SaveAsync();
                return "";
            }
            return "Внутренняя ошибка";
        }
    }
}
