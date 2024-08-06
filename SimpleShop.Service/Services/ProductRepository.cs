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

        public Task AddProduct(Product product)
        {
            throw new NotImplementedException();
        }

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

        public Task DeleteProduct(Product product)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Product>> GetProducts()
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<Product>> GetProducts(ProductFilterDto productFilterDto, bool trackChanges)
        {
            var query = await FindAllAsync(false);
            query = ApplyFilter(query, productFilterDto, false);
            return await query.ToListAsync();
        }

        public Task UpdateProduct(Product product)
        {
            throw new NotImplementedException();
        }
    }
}
