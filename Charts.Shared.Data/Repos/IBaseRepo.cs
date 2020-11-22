using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Charts.Shared.Data.Repos
{
    public interface IBaseRepo<TEntity>
    {
        IQueryable<TEntity> Base();
        IQueryable<TEntity> GetQueryable(Expression<Func<TEntity, bool>> expression);
        Task<Guid> Add(TEntity entity);
        Task AddRange(IEnumerable<TEntity> entity);
        Task Save();
        Task<TEntity> GetById(Guid id);
        Task Delete(TEntity entity);
        Task DeleteRange(IEnumerable<TEntity> entities);
        Task Remove(TEntity entity);
        Task RemoveRange(IEnumerable<TEntity> entities);
        Task Update(TEntity entity);
        Task UpdateRange(IEnumerable<TEntity> entity);
    }
}
