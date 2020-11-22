using Charts.Shared.Data.Mongo.Context;
using Charts.Shared.Data.Mongo.Models;

namespace Charts.Shared.Data.Mongo.Repo
{
    public class VolnaRepo : BaseRepository<Devices>, IVolnaRepo
    {
        public VolnaRepo(IMongoVolnaContext context) : base(context)
        {
        }
    }
}
