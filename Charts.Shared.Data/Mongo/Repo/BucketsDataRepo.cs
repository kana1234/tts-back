using Charts.Shared.Data.Mongo.Context;
using Charts.Shared.Data.Mongo.Models;

namespace Charts.Shared.Data.Mongo.Repo
{
    public class BucketsDataRepo : BaseRepository<buckets_data>, IBucketsDataRepo
    {
        public BucketsDataRepo(IMongoVolnaContext context) : base(context)
        {
        }
    }
}
