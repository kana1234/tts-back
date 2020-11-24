using System;
using System.Threading.Tasks;
using Charts.Shared.Data.Primitives;
using Charts.Shared.Logic.Models.Applciation;

namespace Charts.Shared.Logic.Application
{
    public interface IApplicationLogic
    {
        Task<object> GetApplication(Guid id);
        object GetApplications(LoanApplicationFilter filter, Guid userId);
        Task SetStatus(Guid applicationId, ApplicationTypeEnum status);
        Task<Guid> InsertOrUpdateApplication(ApplicationInDto model, Guid userId);
        Task<object> GetApplicationById(Guid id);
        Task<object> GetApplications(Guid userId);
    }
}