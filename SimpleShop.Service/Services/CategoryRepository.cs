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
    public class CategoryRepository : RepositoryBase<Category> , ICategoryRepository
    {
        public CategoryRepository(RepositoryContext repositoryContext) : base(repositoryContext)
        {
        }

        public async Task AddCategory(Category category)=>
            await CreateAsync(category);

        public async Task DeleteCatigory(Category category)=>
            await RemoveAsync(category);

        public async Task<IEnumerable<Category>> GetCategiries()=>
            await FindAllAsync(false);

        public async Task UpdateCategory(Category category)=>
            await UpdateAsync(category);
    }
}
