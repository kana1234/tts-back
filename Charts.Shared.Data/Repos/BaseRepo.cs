using Charts.Shared.Data.Context;
using Charts.Shared.Data.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Charts.Shared.Data.Repos
{
    /// <summary>
    /// Базовый репозиторий
    /// </summary>
    public class BaseRepo<TEntity> : IBaseRepo<TEntity> where TEntity : BaseEntity
    {
        protected DataContext ContextBase;
        protected DbSet<TEntity> Repo;

        public BaseRepo(DataContext context)
        {
            ContextBase = context;
            Repo = ContextBase.Set<TEntity>();
        }

        public IQueryable<TEntity> Base() => Repo.AsQueryable(); 

        public IQueryable<TEntity> GetQueryable(Expression<Func<TEntity, bool>> expression = null)
        {
            return Repo.Where(expression).AsQueryable();
        }

        public async Task<Guid> Add(TEntity entity)
        {
            await Repo.AddAsync(entity);
            await Save();
            return entity.Id;
        }

        public async Task AddRange(IEnumerable<TEntity> entity)
        {
            await Repo.AddRangeAsync(entity);
            await Save();
        }

        public async Task Save()
        {
            await ContextBase.SaveChangesAsync();
        }

        public async Task<TEntity> GetById(Guid id)
        {
            return await Repo.FindAsync(id);
        }

        public async Task Delete(TEntity entity)
        {
            entity.IsDeleted = true;
            await Save();
        }

        public async Task DeleteRange(IEnumerable<TEntity> entities)
        {
            entities.ToList().ForEach(x => x.IsDeleted = true);
            await Save();
        }

        public async Task Remove(TEntity entity)
        {
            Repo.Remove(entity);
            await Save();
        }

        public async Task RemoveRange(IEnumerable<TEntity> entities)
        {
            Repo.RemoveRange(entities);
            await Save();
        }

        public async Task Update(TEntity entity)
        {
            Repo.Update(entity);
            await Save();
        }

        public async Task Update(TEntity entity,Guid Id)
        {
            entity.Id = Id;
            Repo.Update(entity);
            await Save();
        }

        public async Task UpdateRange(IEnumerable<TEntity> entity)
        {
            Repo.UpdateRange(entity);
            await Save();
        }
    }
}
