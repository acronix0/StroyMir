﻿using Microsoft.EntityFrameworkCore;
using NLog.Filters;
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
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace SimpleShop.Service.Services
{
    public class ProductRepository : RepositoryBase<Product>, IProductRepository
    {
        public ProductRepository(RepositoryContext repositoryContext) : base(repositoryContext)
        {
        }
       
        public async Task<IEnumerable<Product>> GetProducts(CatalogFilterDto filter, bool trackChanges)
        {
            var query = await FindAllAsync(trackChanges);
            query = ApplyFilter(query, filter);
            var result = await query.ToListAsync();
            return result;
        }

        public async Task<IEnumerable<Product>> GetProducts(ProductFilterDto productFilterDto, bool trackChanges)
        {
            var query = await FindAllAsync(trackChanges);
            query = ApplyFilter(query, productFilterDto);
            return await query.ToListAsync();
        }
        public async Task<IEnumerable<Product>> GetProductsById(List<int> ids, bool trackChanges)
        {
            var query = await FindAllAsync(trackChanges);
            query = query.Where(p=>ids.Contains(p.Id));
            return await query.ToListAsync();
        }

        public IQueryable<Product> ApplyFilter(IQueryable<Product> query, CatalogFilterDto filter)
        {
            query = query.Include(p => p.Category);
            if (filter.CategoryId != 0)
            {
                query = query.Where(product => product.Category.Id == filter.CategoryId );
            }
            query = query.Where(product => product.Name.Contains(filter.SearchText));
            if (filter.MaxPrice > 0)
            {
                query = query.Where(q => q.Price >= filter.MinPrice && q.Price <= filter.MaxPrice); //Between(query, q=>q.Price, filter.MinPrice, filter.MaxPrice);
            }
            if (filter.InStock)
            {
                query = query.Where(p => p.Count > 0);
            }
            switch (filter.SortedType)
            {
                case SortedType.name:
                    query = query.OrderBy(p => p.Name);
                    break;
                case SortedType.priceUp:
                    query = query.OrderBy(p => p.Price);
                    break;
                case SortedType.priceDown:
                    query = query.OrderByDescending(p => p.Price);
                    break;
                case SortedType.count:
                    query = query.OrderBy(p => p.Count);
                    break;
                default:
                    break;
            }
            
            query = query.Skip(filter.Skip).Take(filter.Take);
           
            return query;
        }

        public IQueryable<Product> ApplyFilter(IQueryable<Product> query, ProductFilterDto filter)
        {
            query = query.Include(p => p.Category).Where(p => p.Name.Contains(filter.SearchText)).Take(filter.Take);
            return query;
        }
        public async Task<Product> GetProduct(string productArticle)=>
            await FindByConditionAsync(p => p.Article == productArticle).Result.FirstOrDefaultAsync();
        public async Task AddProduct(Product product)=>
            await CreateAsync(product);
        public async Task AddRangeProduct(IEnumerable<Product> products) =>
           await CreateRangeAsync(products);
        public async Task DeleteProduct(Product product)=>
            await RemoveAsync(product);
      
        public Task UpdateProduct(Product product)=>
            UpdateAsync(product);
        public Task UpdateRangeProduct(IEnumerable<Product> products)=>
            UpdateRangeAsync(products);

        public async Task<IEnumerable<Product>> GetProducts()=>
            await FindAllAsync(false);

        public async Task<Product> GetProduct(int id)=>
            await FindByConditionAsync(p => p.Id == id).Result.FirstOrDefaultAsync();
    }
}
