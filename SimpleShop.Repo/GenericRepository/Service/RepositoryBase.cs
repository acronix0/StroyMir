using Microsoft.EntityFrameworkCore.ChangeTracking;
using SimpleShop.Repo.Data;
using SimpleShop.Repo.GenericRepository.Intarface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SimpleShop.Core.Model;
namespace SimpleShop.Repo.GenericRepository.Service
{
    public abstract class RepositoryBase<T> : IRepositoryBase<T> where T :  BaseEntity
    {
        protected RepositoryContext RepositoryContext;
        public RepositoryBase(RepositoryContext repositoryContext) =>
            RepositoryContext = repositoryContext;

        public async Task<IQueryable<T>> FindAllAsync(bool trackChanges) =>
            !trackChanges ? await Task.Run(() => RepositoryContext.Set<T>().AsNoTracking()) : await Task.Run(() => RepositoryContext.Set<T>());

        public virtual async Task<IQueryable<T>> FindByConditionAsync(Expression<Func<T, bool>> expression, bool trackChanges = false) =>
            !trackChanges ? await Task.Run(() => RepositoryContext.Set<T>().Where(expression).AsNoTracking()) : await Task.Run(() => RepositoryContext.Set<T>().Where(expression));
        public IQueryable<T> FindByCondition(Expression<Func<T, bool>> expression, bool trackChanges = false) =>
            !trackChanges ? RepositoryContext.Set<T>().Where(expression).AsNoTracking() : RepositoryContext.Set<T>().Where(expression);

        public async Task<EntityEntry<T>> CreateAsync(T entity) =>
                  await RepositoryContext.Set<T>().AddAsync(entity);

        public async Task UpdateAsync(T entity) =>
            await Task.Run(() => RepositoryContext.Set<T>().Update(entity));
        public async Task RemoveAsync(T entity) =>
             await Task.Run(() => RepositoryContext.Set<T>().Remove(entity));



    }
}
