using SimpleShop.Core.Dtos;
using SimpleShop.Core.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleShop.Service.Interfaces
{
    public interface IProductRepository
    {
        Task<Product> GetProduct(string article);
        Task<Product> GetProduct(int id);
        Task<IEnumerable<Product>> GetProducts();
        Task<IEnumerable<Product>> GetProducts(ProductFilterDto filter, bool trackChanges);
        Task<IEnumerable<Product>> GetProducts(CatalogFilterDto filter, bool trackChanges);
        Task<IEnumerable<Product>> GetProductsById(List<int> ids, bool trackChanges);
        IQueryable<Product> ApplyFilter(IQueryable<Product> query, ProductFilterDto filter);
        IQueryable<Product> ApplyFilter(IQueryable<Product> query, CatalogFilterDto filter);
        Task AddProduct(Product product);
        Task AddRangeProduct(IEnumerable<Product> product);
        Task UpdateProduct(Product product);
        Task DeleteProduct(Product product);
        Task UpdateRangeProduct(IEnumerable<Product> products);

    }
}
