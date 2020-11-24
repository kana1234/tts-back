using Charts.Shared.Data.Context;
using Charts.Shared.Data.Repos;

namespace Charts.Shared.Logic
{
    public interface IBaseLogic
    {
        BaseRepo<T> Of<T>() where T : BaseEntity;
    }
}
