using System;
using System.Linq;
using System.Threading.Tasks;
using Charts.Shared.Data;
using Charts.Shared.Data.Context;
using Charts.Shared.Data.Primitives;
using Charts.Shared.Logic.Dictionary;
using Charts.Shared.Logic.User;
using Microsoft.Extensions.Options;
using WorkflowCore.Interface;

namespace Charts.Shared.Logic.ApplicationTask
{
    public class ApplicationTaskLogic : IApplicationTaskLogic
    {
        private readonly IOptions<AppSettings> _conf;
        private readonly IDictionaryLogic _dictionarylogic;
        private readonly IUserLogic _userLogic;
        private readonly IBaseLogic _baseLogic;


        public ApplicationTaskLogic(IOptions<AppSettings> conf, IDictionaryLogic dictionarylogic, IUserLogic userLogic, IBaseLogic baseLogic)
        {
            _conf = conf;
            _dictionarylogic = dictionarylogic;
            _userLogic = userLogic;
            _baseLogic = baseLogic;
        }
        

        public async Task<Guid> InsertLoanApplicationTaskUser(Guid applicationId, Guid userId, RoleEnum role, ApplicationStatusEnum status,string instanceId = null)
        {
            if (!string.IsNullOrEmpty(instanceId))
            {
                var application =await _baseLogic.Of<Data.Context.Application>().GetById(applicationId);
                application.Status = status;
                await _baseLogic.Of<Data.Context.Application>().Update(application);
            }
            var planEndDate = await GetPlanedEndDatetime(applicationId, status);
            return await _baseLogic.Of<Data.Context.ApplicationTask>().Add(
                new Data.Context.ApplicationTask
                {
                    ApplicationId = applicationId,
                    UserId = userId,
                    RoleId = _baseLogic.Of<Role>().GetQueryable(x => !x.IsDeleted && x.Value == role).Select(x => x.Id).FirstOrDefault(),
                    AppointmentDate = DateTime.Now,
                    PlanEndDate = planEndDate,
                    Status = status
                });
        }

        public async Task InsertLoanApplicationTaskUserPrevious(Guid applicationId, Guid userId, RoleEnum role, ApplicationStatusEnum status)
        {
            var planEndDate = await GetPlanedEndDatetime(applicationId, status);
            await _baseLogic.Of<Data.Context.ApplicationTask>().Add(
                new Data.Context.ApplicationTask
                {
                    ApplicationId = applicationId,
                    UserId = userId,
                    RoleId = _baseLogic.Of<Role>().GetQueryable(x => !x.IsDeleted && x.Value == role).Select(x => x.Id).FirstOrDefault(),
                    AppointmentDate = DateTime.Now,
                    PlanEndDate = planEndDate,
                    Status = status
                });
        }

        private async Task<DateTime?> GetPlanedEndDatetime(Guid applicationId,ApplicationStatusEnum status)
        {
            var application = await _baseLogic.Of<Data.Context.Application>().GetById(applicationId);
            switch (status)
            {
                case ApplicationStatusEnum.InWork:
                {
                    if (application.WithReplacement)
                        return DateTime.Now.AddDays(19);
                    else
                        return DateTime.Now.AddDays(9);
                }
                case ApplicationStatusEnum.DocumentCollect:
                    return DateTime.Now.AddDays(10);

                case ApplicationStatusEnum.ReWork:
                    return DateTime.Now.AddDays(3);

                case ApplicationStatusEnum.Agreement:
                    return DateTime.Now.AddDays(2);

                default:
                    return null;
            }
        }

        public async Task DeleteApplicationTask(Guid taskApplicationId, ApplicationTaskStatusEnum taksStatus, string comment)
        {
            var task = await _baseLogic.Of<Data.Context.ApplicationTask>().GetById(taskApplicationId);
            await _baseLogic.Of<ApplicationHistory>().Add(
                new ApplicationHistory
                {
                    Id = task.Id,
                    CreatedDate = task.CreatedDate,
                    ModifiedDate = task.ModifiedDate,
                    IsDeleted = task.IsDeleted,
                    ApplicationId = task.ApplicationId,
                    Status = taksStatus,
                    UserId = task.UserId,
                    AppointmentDate = task.AppointmentDate,
                    PlanEndDate = task.PlanEndDate,
                    FactEndDate = DateTime.Now,
                    Comment = comment
                });
            await _baseLogic.Of<Data.Context.ApplicationTask>().Remove(task);
        }


    }
}