using SimpleShop.Core.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleShop.Service.Interfaces
{
    public interface ICategoryRepository
    {
        Task AddCategory(Category category);

        Task<IEnumerable<Category>> GetCategiries();
        Task UpdateCategory(Category category);
        Task DeleteCatigory(Category category);
        


    }
}
