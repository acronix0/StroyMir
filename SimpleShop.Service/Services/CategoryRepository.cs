using Microsoft.EntityFrameworkCore;
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
    public class CategoryRepository : RepositoryBase<Category>, ICategoryRepository
    {
        public CategoryRepository(RepositoryContext repositoryContext) : base(repositoryContext)
        {
        }

        public async Task AddCategory(Category category) =>
            await CreateAsync(category);
        public async Task AddRangeCategory(IEnumerable<Category> categories)=>
            await CreateRangeAsync(categories);
        public async Task DeleteCatigory(Category category) =>
            await RemoveAsync(category);

        public async Task<IEnumerable<Category>> GetCategories()=>
            await FindAllAsync(false);

        public async Task<Category> GetCategory(int id) => 
            await FindByConditionAsync(c => c.Id == id).Result.FirstOrDefaultAsync();

        public async Task UpdateCategory(Category category)=>
            await UpdateAsync(category);
        public async Task UpdateRangeCategory( IEnumerable<Category> categories) =>
            await UpdateRangeAsync(categories);

    }
}
