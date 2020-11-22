using Charts.Shared.Data.Mongo.Models;
using Charts.Shared.Data.Mongo.Primitives;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Charts.Shared.Data.Mongo.Context
{
    public class MongoVolnaContext : IMongoVolnaContext
    {
        private IMongoDatabase Database { get; set; }
        public IClientSessionHandle Session { get; set; }
        public MongoClient MongoClient { get; set; }
        private readonly List<Func<Task>> _commands;
        private readonly IOptions<AppSettings> _configuration;

        public MongoVolnaContext(IOptions<AppSettings> configuration)
        {
            _configuration = configuration;
            _commands = new List<Func<Task>>();
        }

        //public async Task<int> SaveChanges()
        //{
        //    ConfigureMongo();
        //    using (Session = await MongoClient.StartSessionAsync())
        //    {
        //        Session.StartTransaction();
        //        var commandTasks = _commands.Select(c => c());
        //        await Task.WhenAll(commandTasks);
        //        await Session.CommitTransactionAsync();
        //    }
        //    return _commands.Count;
        //}

        private void ConfigureMongo(MongoDbType type)
        {
            if (MongoClient != null)
            {
                return;
            }

            MongoClient = new MongoClient(_configuration.Value.MongoConfig.ConnectionString);
            Database = MongoClient.GetDatabase(
                type == MongoDbType.Volna
                ? _configuration.Value.MongoConfig.Database
                : _configuration.Value.MongoConfig.Database2);
        }

        public IMongoCollection<T> GetCollection<T>(string name)
        {
            if(name == nameof(buckets_data))
                ConfigureMongo(MongoDbType.VolnaData);
            else
                ConfigureMongo(MongoDbType.Volna);
            return Database.GetCollection<T>(name);
        }

        public void Dispose()
        {
            Session?.Dispose();
            GC.SuppressFinalize(this);
        }

        public void AddCommand(Func<Task> func)
        {
            _commands.Add(func);
        }
    }
}
