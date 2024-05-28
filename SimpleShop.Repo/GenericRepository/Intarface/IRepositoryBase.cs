using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace SimpleShop.Repo.GenericRepository.Intarface
{
    public interface IRepositoryBase<T> where T : class
    {
        
        Task<EntityEntry<T>> CreateAsync(T entity);
        Task UpdateAsync(T entity);
        Task RemoveAsync(T entity);
    }
}
