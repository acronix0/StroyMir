using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using SimpleShop.Core.Model;
using SimpleShop.Repo.Data;
using SimpleShop.Repo.GenericRepository.Service;
using SimpleShop.Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace SimpleShop.Service.Services
{
#pragma warning disable CS8603 // Возможно, возврат ссылки, допускающей значение NULL.
    public class BaseEntityRepository<T> : RepositoryBase<BaseEntity> where T : BaseEntity
    {
        public BaseEntityRepository(RepositoryContext repositoryContext) : base(repositoryContext)
        {
        }

        public Task<EntityEntry<T>> AddEntity(T baseEntity) =>
            CreateAsync(baseEntity) as Task<EntityEntry<T>>;

        public async Task<IEnumerable<T>> GetEntities(bool trackChanges) =>
            await FindAllAsync(trackChanges).Result.ToListAsync() as IEnumerable<T>;

        public async Task UpdateEntity(BaseEntity baseEntity) =>
            await UpdateAsync(baseEntity);

        Task<T> GetEntity(int id, bool trackChanges)
        {
            var query = FindByConditionAsync(b => b.Id == id, trackChanges);
            return query.Result.SingleAsync() as Task<T>;
        }

        public Task DeleteEntity(BaseEntity baseEntity)=>
            RemoveAsync(baseEntity);


    }
}
#pragma warning restore CS8603 // Возможно, возврат ссылки, допускающей значение NULL.
