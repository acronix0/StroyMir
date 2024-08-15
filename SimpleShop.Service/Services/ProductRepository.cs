using Microsoft.EntityFrameworkCore;
using SimpleShop.Core.Dtos;
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
    public class ProductRepository : RepositoryBase<Product>, IProductRepository
    {
        public ProductRepository(RepositoryContext repositoryContext) : base(repositoryContext)
        {
        }
        public async Task<Product> GetProduct(string productArticle)=>
            await FindByConditionAsync(p => p.Article == productArticle).Result.SingleAsync();
        public async Task AddProduct(Product product)=>
            await CreateAsync(product);
        public async Task AddRangeProduct(IEnumerable<Product> products) =>
           await CreateRangeAsync(products);
        public IQueryable<Product> ApplyFilter(IQueryable<Product> query, ProductFilterDto filter, bool trackChanges)
        {
            if (filter == null && filter.SearchText == null)
                return query;
            switch (filter.Property)
            {
                case ProductsPropertySearch.Name:
                    query = query.Where(product => product.Name.Contains(filter.SearchText));
                    break;
                case ProductsPropertySearch.Category:
                    query = query.Where(product => product.Category.Name.Contains(filter.SearchText));
                    break;
            }
            return query;
        }

        public async Task DeleteProduct(Product product)=>
            await RemoveAsync(product);

        public async Task<IEnumerable<Product>> GetProducts()=>
            await FindAllAsync(false);


        public async Task<IEnumerable<Product>> GetProducts(ProductFilterDto productFilterDto, bool trackChanges)
        {
            var query = await FindAllAsync(false);
            query = ApplyFilter(query, productFilterDto, false);
            return await query.ToListAsync();
        }

        public Task UpdateProduct(Product product)=>
            UpdateAsync(product);
        public Task UpdateRangeProduct(IEnumerable<Product> products)=>
            UpdateRangeAsync(products);
    }
}
