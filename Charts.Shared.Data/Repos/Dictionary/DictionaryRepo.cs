using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Charts.Shared.Data.Context;
using Charts.Shared.Data.Context.Dictionary;
using Microsoft.EntityFrameworkCore;

namespace Charts.Shared.Data.Repos.Dictionary
{
    public class DictionaryRepo<TEntity> : IDictionaryRepo<TEntity> where TEntity : BaseDictionary
    {
        private readonly DbSet<TEntity> _objectSet = null;
        private readonly DataContext _context;
        public DictionaryRepo(DataContext context)
        {
            _objectSet = context.Set<TEntity>();
            _context = context;
        }

        public DictionaryRepo(DbSet<TEntity> objectSet)
        {
            _objectSet = objectSet;
        }

        public IQueryable<TEntity> GetQueryable(Expression<Func<TEntity, bool>> expression = null)
        {
            if (expression != null)
            {
                return _objectSet.Where(expression);
            }
            return _objectSet.AsQueryable<TEntity>();
        }

        public async Task<TEntity> GetById(Guid id)
        {
            return await _objectSet.FindAsync(id);
        }

        public async Task<TEntity> Add(TEntity entity)
        {
            await _objectSet.AddAsync(entity);
            await Save();
            return entity;
        }


        public async Task Update(TEntity entity)
        {
            _context.Update(entity);
            await Save();
        }

        public async Task UpdateRange(IEnumerable<TEntity> entity)
        {
            _context.UpdateRange(entity);
            await Save();
        }

        public async Task Save()
        {
            await _context.SaveChangesAsync();
        }

        public IQueryable<TEntity> Base() => _objectSet.AsQueryable();

        async Task<Guid> IBaseRepo<TEntity>.Add(TEntity entity)
        {
            await _objectSet.AddAsync(entity);
            await Save();
            return entity.Id;
        }

        public async Task AddRange(IEnumerable<TEntity> entity)
        {
            await _objectSet.AddRangeAsync(entity);
            await Save();
        }

        public Task Delete(TEntity entity)
        {
            throw new NotImplementedException();
        }

        public Task DeleteRange(IEnumerable<TEntity> entities)
        {
            throw new NotImplementedException();
        }

        public Task Remove(TEntity entity)
        {
            throw new NotImplementedException();
        }

        public Task RemoveRange(IEnumerable<TEntity> entities)
        {
            throw new NotImplementedException();
        }


    }
}
