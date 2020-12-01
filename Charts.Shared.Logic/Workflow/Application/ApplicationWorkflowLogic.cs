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
            var roles = await _baseLogic.Of<Shared.Data.Context.User>().Base().Include(a=>a.UserRoles).ThenInclude(a=>a.Role).FirstOrDefaultAsync(a=>a.Id==userId);
            return roles.Roles.FirstOrDefault()?.Value;

        }

        protected async Task<Shared.Data.Context.ApplicationTask> GetCurrentUserTask(Guid userId,Guid appId)
        {
            var task = await _baseLogic.Of<Shared.Data.Context.ApplicationTask>().Base().FirstOrDefaultAsync(a=>a.UserId==userId && a.ApplicationId== appId && !a.IsDeleted);
            return task;

        }

        protected async Task<Shared.Data.Context.Application> GetApplicationById(Guid applicationId)
        {
            var application = await _baseLogic.Of<Shared.Data.Context.Application>().Base().FirstOrDefaultAsync(a => a.Id == applicationId && !a.IsDeleted);
            return application;

        }
        protected Guid GetUserIdByApplicationIdAndRole(Guid applicationId, RoleEnum role)
        {
            var _ = _baseLogic.Of<Shared.Data.Context.User>().Base()
                .Include(a=>a.UserRoles).ThenInclude(a=>a.Role)
                .FirstOrDefaultAsync(a => a.UserRoles.Any(t=>t.Role.Value ==role )).Result;
            if (_ == null || _.Id == null)
                throw new Exception("Не удалось найти пользователей");
            return _.Id;
        }

        protected Guid GetAuditServiceOrEconomistByApplicationId(Guid applicationId)
        {
            var task =  _baseLogic.Of<Shared.Data.Context.ApplicationTask>().Base()
                .FirstOrDefaultAsync(a =>
                    a.ApplicationId == applicationId && (a.Role.Value == RoleEnum.SalesDepartment || a.Role.Value == RoleEnum.AuditService)).Result;
            if (task == null || task.UserId == null)
                throw new Exception("Не удалось найти пользователей");
            return (Guid)task.UserId;
        }

        protected Guid GetUserByContractorId(Guid? contractorId)
        {
            var user = _baseLogic.Of<Shared.Data.Context.User>().Base()
                .FirstOrDefaultAsync(a => a.ContractorId == contractorId
                   ).Result;
            if (user == null || user.Id == null)
                throw new Exception("Не удалось найти пользователей");
            return user.Id;
        }



        public async Task SendApplicationToUser(ApplicationStatusInDto model, Guid currentUserId)
        {
            var role = await GetRoleByUserId(currentUserId);
            var task = await GetCurrentUserTask(currentUserId, model.ApplicationId);
            var taskId = task.Id;
            var application = await GetApplicationById(model.ApplicationId);

            try
            {
                if (model.Decision == ApplicationTaskStatusEnum.SUCCESS)
                {
                    switch (role)
                    {
                        case RoleEnum.TorSpecialist:
                            await _applicationTaskLogic.InsertLoanApplicationTaskUser(application, GetUserByContractorId(application.ContractorsId), RoleEnum.Counterparty,
                                ApplicationStatusEnum.InRepair);
                            break;
                        case RoleEnum.Counterparty when task.Status == ApplicationStatusEnum.InRepair:
                            await _applicationTaskLogic.InsertLoanApplicationTaskUser(application, currentUserId, RoleEnum.Counterparty,
                                ApplicationStatusEnum.DocumentCollect);
                            break;

                        case RoleEnum.Counterparty when task.Status == ApplicationStatusEnum.DocumentCollect && application.RepairType == RepairType.Complaint:
                            await _applicationTaskLogic.InsertLoanApplicationTaskUser(application, GetUserIdByApplicationIdAndRole(model.ApplicationId, RoleEnum.AuditService),
                                RoleEnum.AuditService,
                                ApplicationStatusEnum.Agreement);
                            break;
                        case RoleEnum.Counterparty when task.Status == ApplicationStatusEnum.DocumentCollect && application.RepairType == RepairType.Operational:
                            await _applicationTaskLogic.InsertLoanApplicationTaskUser(application, GetUserIdByApplicationIdAndRole(model.ApplicationId, RoleEnum.SalesDepartment), RoleEnum.SalesDepartment,
                                ApplicationStatusEnum.Agreement);
                            break;
                        /////
                        case RoleEnum.Counterparty when task.Status == ApplicationStatusEnum.ReWork && application.RepairType == RepairType.Complaint:
                            await _applicationTaskLogic.InsertLoanApplicationTaskUser(application, GetAuditServiceOrEconomistByApplicationId(model.ApplicationId), RoleEnum.AuditService,
                                ApplicationStatusEnum.Agreement);
                            break;
                        case RoleEnum.Counterparty when task.Status == ApplicationStatusEnum.ReWork && application.RepairType == RepairType.Operational:
                            await _applicationTaskLogic.InsertLoanApplicationTaskUser(application, GetAuditServiceOrEconomistByApplicationId(model.ApplicationId), RoleEnum.SalesDepartment,
                                ApplicationStatusEnum.Agreement);
                            break;
                        //////
                        case RoleEnum.AuditService:
                            await _applicationTaskLogic.InsertLoanApplicationTaskUser(application, GetUserIdByApplicationIdAndRole(model.ApplicationId, RoleEnum.Economist), RoleEnum.Economist,
                                ApplicationStatusEnum.Agreement);
                            break;

                        case RoleEnum.SalesDepartment:
                            await _applicationTaskLogic.InsertLoanApplicationTaskUser(application, GetUserIdByApplicationIdAndRole(model.ApplicationId, RoleEnum.Economist), RoleEnum.Economist,
                                ApplicationStatusEnum.Agreement);
                            break;

                        case RoleEnum.Economist:
                            await _applicationTaskLogic.InsertLoanApplicationTaskUser(application, GetUserIdByApplicationIdAndRole(model.ApplicationId, RoleEnum.TorManager), RoleEnum.TorManager,
                                ApplicationStatusEnum.PaymentFormation);
                            break;

                        case RoleEnum.TorManager:
                            await _applicationTaskLogic.InsertLoanApplicationTaskUser(application, GetUserIdByApplicationIdAndRole(model.ApplicationId, RoleEnum.Treasurer), RoleEnum.Treasurer,
                                ApplicationStatusEnum.Payment);
                            break;

                        case RoleEnum.Treasurer:
                            await _applicationTaskLogic.InsertLoanApplicationTaskUser(application, currentUserId, RoleEnum.Treasurer,
                                ApplicationStatusEnum.Paid);
                            break;
                    }
                }
                else
                {
                    var conterpartyUser = await _baseLogic.Of<Shared.Data.Context.ApplicationTask>().Base()
                        .FirstOrDefaultAsync(a =>
                            a.ApplicationId == model.ApplicationId && a.Role.Value == RoleEnum.Counterparty);
                    if(conterpartyUser==default)
                        throw new Exception("Не указан Контрагент");

                    if (conterpartyUser.UserId != null)
                        await _applicationTaskLogic.InsertLoanApplicationTaskUser(application,
                            (Guid) conterpartyUser.UserId, RoleEnum.Counterparty,
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
        public async Task CreateApplication(Guid applicationId, Guid currentUserId)
        {
            try
            {
                var application = await GetApplicationById(applicationId);
                await _applicationTaskLogic.InsertLoanApplicationTaskUser(application, currentUserId,
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