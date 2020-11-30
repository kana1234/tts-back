using System;
using System.Threading.Tasks;
using Charts.Shared.Data.Primitives;
using Charts.Shared.Logic.Models.Applciation;

namespace Charts.Shared.Logic.Workflow.Application
{
    public interface IApplicationWorkflowLogic
    {
        Task CreateApplication(Guid appplicationId, Guid currentUserId);
        Task SendApplicationToUser(ApplicationStatusInDto model, Guid currentUserId);
    }
}