using MongoDB.Driver;
using System;
using System.Threading.Tasks;

namespace Charts.Shared.Data.Mongo.Context
{
    public interface IMongoVolnaContext : IDisposable
    {
        void AddCommand(Func<Task> func);
        //Task<int> SaveChanges();
        IMongoCollection<T> GetCollection<T>(string name);
    }
}
