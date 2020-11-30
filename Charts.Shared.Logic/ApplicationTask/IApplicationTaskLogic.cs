using System;
using System.Threading.Tasks;
using Charts.Shared.Data.Primitives;

namespace Charts.Shared.Logic.ApplicationTask
{
    public interface IApplicationTaskLogic
    {
        Task<Guid> InsertLoanApplicationTaskUser(Guid applicationId, Guid userId, RoleEnum role,
            ApplicationStatusEnum status, string instanceId = null);
        Task InsertLoanApplicationTaskUserPrevious(Guid applicationId,  Guid userId, RoleEnum role, ApplicationStatusEnum status);
        Task DeleteApplicationTask(Guid taskApplicationId, ApplicationTaskStatusEnum taksStatus, string comment);
    }
}