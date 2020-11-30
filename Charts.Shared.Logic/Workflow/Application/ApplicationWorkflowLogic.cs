using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper.Configuration.Annotations;
using Charts.Shared.Data;
using Charts.Shared.Data.Context;
using Charts.Shared.Data.Primitives;
using Charts.Shared.Logic.ApplicationTask;
using Charts.Shared.Logic.Dictionary;
using Charts.Shared.Logic.Models.Applciation;
using Charts.Shared.Logic.User;
using Charts.Shared.Logic.Workflow.Data;
using Charts.Shared.Logic.Workflow.Steps;
using Microsoft.AspNetCore.Server.IIS.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using WorkflowCore.Interface;

namespace Charts.Shared.Logic.Workflow.Application
{
    public class ApplicationWorkflowLogic : IApplicationWorkflowLogic
    {
        private readonly IOptions<AppSettings> _conf;
        private readonly IBaseLogic _baseLogic;
        //private readonly IWorkflowController _workflowService;
        //private readonly IWorkflowHost _workflowHost;
        //private readonly IWorkflowRegistry _registry;
        //private readonly IPersistenceProvider _workflowStore;
        //private readonly ISearchIndex _searchService;
        private readonly IApplicationTaskLogic _applicationTaskLogic;


        public ApplicationWorkflowLogic(IOptions<AppSettings> conf, IBaseLogic baseLogic, IApplicationTaskLogic applicationTaskLogic)
        {
            _conf = conf;
            _baseLogic = baseLogic;
            _applicationTaskLogic = applicationTaskLogic;
        }

        protected async Task<RoleEnum?> GetRoleByUserId(Guid userId)
        {
            var roles = await _baseLogic.Of<Shared.Data.Context.User>().Base().FirstOrDefaultAsync(a=>a.Id==userId);
            return roles.Roles.FirstOrDefault()?.Value;

        }

        protected async Task<Shared.Data.Context.ApplicationTask> GetCurrentUserTask(Guid userId)
        {
            var task = await _baseLogic.Of<Shared.Data.Context.ApplicationTask>().Base().FirstOrDefaultAsync(a=>a.UserId==userId && !a.IsDeleted);
            return task;

        }

        protected async Task<Shared.Data.Context.Application> GetApplicationById(Guid applicationId)
        {
            var application = await _baseLogic.Of<Shared.Data.Context.Application>().Base().FirstOrDefaultAsync(a => a.Id == applicationId && !a.IsDeleted);
            return application;

        }
        protected Guid GetUserIdByApplicationIdAndRole(Guid applicationId, RoleEnum role)
        {
            var task = _baseLogic.Of<Shared.Data.Context.ApplicationTask>().Base()
                .FirstOrDefaultAsync(a => a.ApplicationId == applicationId && (a.Role.Value == role)).Result;
            if (task == null)
                task = _baseLogic.Of<Shared.Data.Context.ApplicationTask>().Base()
                    .FirstOrDefaultAsync(a => (a.Role.Value == role)).Result;
            if (task == null || task.UserId == null)
                throw new Exception("Не удалось найти пользователей");
            return (Guid)task.UserId;
        }

        protected Guid GetAuditServiceOrEconomistByApplicationId(Guid applicationId)
        {
            var task =  _baseLogic.Of<Shared.Data.Context.ApplicationTask>().Base()
                .FirstOrDefaultAsync(a =>
                    a.ApplicationId == applicationId && (a.Role.Value == RoleEnum.Economist || a.Role.Value == RoleEnum.AuditService)).Result;
            if (task == null || task.UserId == null)
                throw new Exception("Не удалось найти пользователей");
            return (Guid)task.UserId;
        }

        public async Task SendApplicationToUser(ApplicationStatusInDto model, Guid currentUserId)
        {
            var role = await GetRoleByUserId(currentUserId);
            var task = await GetCurrentUserTask(currentUserId);
            var taskId = task.Id;
            var application = await GetApplicationById(model.ApplicationId);

            try
            {
                if (model.Decision == ApplicationTaskStatusEnum.SUCCESS)
                {
                    if (model.UserId != null)
                    {
                        switch (role)
                        {
                            case RoleEnum.TorSpecialist:
                                await _applicationTaskLogic.InsertLoanApplicationTaskUser(model.ApplicationId, (Guid)model.UserId, RoleEnum.Counterparty, 
                                    ApplicationStatusEnum.InRepair);
                                break;
                            case RoleEnum.Counterparty when task.Status == ApplicationStatusEnum.InRepair:
                                await _applicationTaskLogic.InsertLoanApplicationTaskUser(model.ApplicationId, currentUserId, RoleEnum.Counterparty,
                                    ApplicationStatusEnum.DocumentCollect);
                                break;

                            case RoleEnum.Counterparty when task.Status == ApplicationStatusEnum.DocumentCollect && application.RepairType == RepairType.Complaint:
                                await _applicationTaskLogic.InsertLoanApplicationTaskUser(model.ApplicationId, GetUserIdByApplicationIdAndRole(model.ApplicationId, RoleEnum.AuditService),
                                    RoleEnum.AuditService,
                                    ApplicationStatusEnum.Agreement);
                                break;
                            case RoleEnum.Counterparty when task.Status == ApplicationStatusEnum.DocumentCollect && application.RepairType == RepairType.Operational:
                                await _applicationTaskLogic.InsertLoanApplicationTaskUser(model.ApplicationId, GetUserIdByApplicationIdAndRole(model.ApplicationId, RoleEnum.SalesDepartment), RoleEnum.SalesDepartment,
                                    ApplicationStatusEnum.Agreement);
                                break;
                                /////
                            case RoleEnum.Counterparty when task.Status == ApplicationStatusEnum.ReWork && application.RepairType == RepairType.Complaint:
                                await _applicationTaskLogic.InsertLoanApplicationTaskUser(model.ApplicationId,  GetAuditServiceOrEconomistByApplicationId(model.ApplicationId), RoleEnum.AuditService,
                                    ApplicationStatusEnum.Agreement);
                                break;
                            case RoleEnum.Counterparty when task.Status == ApplicationStatusEnum.ReWork && application.RepairType == RepairType.Operational:
                                await _applicationTaskLogic.InsertLoanApplicationTaskUser(model.ApplicationId, GetAuditServiceOrEconomistByApplicationId(model.ApplicationId), RoleEnum.Economist,
                                    ApplicationStatusEnum.Agreement);
                                break;
                                //////
                            case RoleEnum.AuditService:
                                await _applicationTaskLogic.InsertLoanApplicationTaskUser(model.ApplicationId, GetUserIdByApplicationIdAndRole(model.ApplicationId, RoleEnum.Economist), RoleEnum.Economist,
                                    ApplicationStatusEnum.Agreement);
                                break;

                            case RoleEnum.SalesDepartment:
                                await _applicationTaskLogic.InsertLoanApplicationTaskUser(model.ApplicationId, GetUserIdByApplicationIdAndRole(model.ApplicationId, RoleEnum.Economist), RoleEnum.Economist,
                                    ApplicationStatusEnum.Agreement);
                                break;

                            case RoleEnum.Economist:
                                await _applicationTaskLogic.InsertLoanApplicationTaskUser(model.ApplicationId, GetUserIdByApplicationIdAndRole(model.ApplicationId, RoleEnum.TorManager), RoleEnum.TorManager,
                                    ApplicationStatusEnum.PaymentFormation);
                                break;

                            case RoleEnum.TorManager:
                                await _applicationTaskLogic.InsertLoanApplicationTaskUser(model.ApplicationId, GetUserIdByApplicationIdAndRole(model.ApplicationId, RoleEnum.Treasurer), RoleEnum.Treasurer,
                                    ApplicationStatusEnum.Payment);
                                break;

                            case RoleEnum.Treasurer:
                                taskId = await _applicationTaskLogic.InsertLoanApplicationTaskUser(model.ApplicationId, currentUserId, RoleEnum.Treasurer,
                                    ApplicationStatusEnum.Paid);
                                break;
                        }
                    }
                    else
                        throw new Exception("Не указан конечный пользователь");
                }
                else
                {
                    var conterpartyUser = await _baseLogic.Of<Shared.Data.Context.ApplicationTask>().Base()
                        .FirstOrDefaultAsync(a =>
                            a.ApplicationId == model.ApplicationId && a.Role.Value == RoleEnum.Counterparty);
                    if(conterpartyUser==default)
                        throw new Exception("Не указан Контрагент");

                    await _applicationTaskLogic.InsertLoanApplicationTaskUser(model.ApplicationId, conterpartyUser.Id, RoleEnum.Counterparty,
                        ApplicationStatusEnum.ReWork);
                }
                await _applicationTaskLogic.DeleteApplicationTask(taskId, model.Decision, model.Comment);

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

        }
        public async Task CreateApplication(Guid appplicationId, Guid currentUserId)
        {
            try
            {
                await _applicationTaskLogic.InsertLoanApplicationTaskUser(appplicationId, currentUserId,
                    RoleEnum.TorSpecialist, ApplicationStatusEnum.Draft);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

            //IServiceCollection services = new ServiceCollection();
            //services.AddTransient<CreateAppStep>();
            //services.AddTransient<IApplicationTaskLogic, IApplicationTaskLogic>();

            //_workflowHost.RegisterWorkflow<Workflows, ApplicationData>();
            //_workflowHost.Start();



            //var instanceId = _workflowService.StartWorkflow(_conf.Value.WorkflowConfig.WorkflowDefinitionId,
            //    _conf.Value.WorkflowConfig.Version, data).Result;

            //if (!string.IsNullOrEmpty(instanceId))
            //{
            //    var application = await _baseLogic.Of<Shared.Data.Context.Application>().GetById(model.ApplicationId);
            //    application.InstanceId = instanceId;
            //    await _baseLogic.Of<Shared.Data.Context.Application>().Update(application);
            //}

        }


    }
}