using System;
using System.Threading.Tasks;

namespace Charts.Logic.Application
{
    public interface IApplicationLogic : Charts.Shared.Logic.Application.IApplicationLogic
    {
        Task ManagerSend(Guid userId, Guid applicationTaskId, string decision, string comment);
    }
}
