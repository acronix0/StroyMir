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
using Microsoft.EntityFrameworkCore.Query;
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
        public virtual async Task<IQueryable<T>> FindByConditionAsyncWithIncludeCollection<TIncludeProperty, TThenIncludeProperty>(
            Expression<Func<T, bool>> expression,
            Expression<Func<T, IEnumerable<TIncludeProperty>>> IncludeExpression,
            Expression<Func<TIncludeProperty, TThenIncludeProperty>> ThenIncludeExpression,
            bool trackChanges = false)
        {
            var query = await Task.Run(() => RepositoryContext.Set<T>()
                                                    .Include(IncludeExpression)
                                                    .ThenInclude(ThenIncludeExpression)
                                                    .Where(expression));
            return trackChanges ? query : query.AsNoTracking();
        }
        public virtual async Task<IQueryable<T>> FindByConditionAsyncWithInclude<TIncludeProperty>(
            Expression<Func<T, bool>> expression,
            Expression<Func<T, TIncludeProperty>> IncludeExpression,
            bool trackChanges = false)
        {
            var query = await Task.Run(() => RepositoryContext.Set<T>()
                                                    .Include(IncludeExpression)
                                                    .Where(expression));
            return trackChanges ? query : query.AsNoTracking();
        }
        public IQueryable<T> FindByCondition(Expression<Func<T, bool>> expression, bool trackChanges = false) =>
            !trackChanges ? RepositoryContext.Set<T>().Where(expression).AsNoTracking() : RepositoryContext.Set<T>().Where(expression);

        public async Task<EntityEntry<T>> CreateAsync(T entity) =>
                  await RepositoryContext.Set<T>().AddAsync(entity);
        public async Task CreateRangeAsync(IEnumerable<T> entities) =>
                  await RepositoryContext.Set<T>().AddRangeAsync(entities);
        public async Task UpdateAsync(T entity) =>
            await Task.Run(() => RepositoryContext.Set<T>().Update(entity));
        public async Task UpdateRangeAsync(IEnumerable<T> entity) =>
            await Task.Run(() => RepositoryContext.Set<T>().UpdateRange(entity));
        public async Task RemoveAsync(T entity) =>
             await Task.Run(() => RepositoryContext.Set<T>().Remove(entity));
        public static IQueryable<T> Between<TKey>( IQueryable<T> query, Expression<Func<T, TKey>> keySelector, TKey low, TKey high) where TKey : IComparable<TKey>
        {
            Expression key = Expression.Invoke(keySelector,
                 keySelector.Parameters.ToArray());
            Expression lowerBound = Expression.GreaterThanOrEqual
                (key, Expression.Constant(low));
            Expression upperBound = Expression.LessThanOrEqual
                (key, Expression.Constant(high));
            Expression and = Expression.AndAlso(lowerBound, upperBound);
            Expression<Func<T, bool>> lambda =
                Expression.Lambda<Func<T, bool>>(and, keySelector.Parameters);
            return query.Where(lambda);
        }
    }
}
