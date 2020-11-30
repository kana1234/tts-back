using System;
using System.Threading.Tasks;
using Charts.Shared.Data.Primitives;
using Charts.Shared.Logic.Models.Applciation;

namespace Charts.Shared.Logic.Application
{
    public interface IApplicationLogic
    {
        Task<object> GetApplication(Guid id);
        Task<object> Statistics(Guid userId);
        Task DeleteApplication(Guid userId,Guid applicationId);
        Task DeleteRemark(Guid id);
        //object GetApplications(LoanApplicationFilter filter, Guid userId);
        //Task SetStatus(Guid applicationId, ApplicationTypeEnum status);
        Task<Guid> InsertOrUpdateApplication(ApplicationInDto model, Guid userId);
        Task<Guid> InsertOrUpdateRemark(RemarkInDto model, Guid userId);

        Task<object> GetApplications(Guid userId, ApplicationStatusEnum status);

        Task<object> GetRemarks(Guid applicationId);
        Task<object> GetContractorsUsers();
    }
}