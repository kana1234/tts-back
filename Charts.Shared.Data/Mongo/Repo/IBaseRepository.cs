using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Charts.Shared.Data.Mongo.Repo
{
    public interface IBaseRepository<TEntity> : IDisposable where TEntity : class
    {
        IFindFluent<TEntity, TEntity> Find(FilterDefinition<TEntity> filter);
        Task<TEntity> GetById(Guid id);
        Task<IEnumerable<TEntity>> GetAll();
    }
}
