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
        Task<IEnumerable<Product>> GetProducts();
        Task<IEnumerable<Product>> GetProducts(ProductFilterDto productFilterDto, bool trackChanges);
        IQueryable<Product> ApplyFilter(IQueryable<Product> query, ProductFilterDto filter, bool trackChanges);
        Task AddProduct(Product product);
        Task AddRangeProduct(IEnumerable<Product> product);
        Task UpdateProduct(Product product);
        Task DeleteProduct(Product product);
        Task UpdateRangeProduct(IEnumerable<Product> products);

    }
}
