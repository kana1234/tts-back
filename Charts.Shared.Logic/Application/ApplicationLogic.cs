using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Camunda.Api.Client;
using Camunda.Api.Client.UserTask;
using Charts.Shared.Data.Context;
using Charts.Shared.Data.Extensions;
using Charts.Shared.Data.Primitives;
using Charts.Shared.Logic.Dictionary;
using Charts.Shared.Logic.Helper;
using Charts.Shared.Logic.Models.Applciation;
using Charts.Shared.Logic.User;
using Charts.Shared.Logic.Workflow.Application;
using Microsoft.EntityFrameworkCore;
using WorkflowCore.Primitives;

namespace Charts.Shared.Logic.Application
{
    public class ApplicationLogic : IApplicationLogic
    {

        private readonly IDictionaryLogic _dictionarylogic;
        private readonly IUserLogic _userLogic;
        private readonly IApplicationWorkflowLogic _applicationWorkflowLogic;
        private readonly IBaseLogic _baseLogic;

        public ApplicationLogic(IDictionaryLogic dictionarylogic, IUserLogic userLogic, IApplicationWorkflowLogic applicationWorkflowLogic, IBaseLogic baseLogic)
        {
            _dictionarylogic = dictionarylogic;
            _userLogic = userLogic;
            _applicationWorkflowLogic = applicationWorkflowLogic;
            _baseLogic = baseLogic;
        }

        public async Task<Guid> InsertOrUpdateApplication(ApplicationInDto model, Guid userId)
        {
            var actions = new List<Action<ApplicationInDto, Data.Context.Application>>();
            actions.Add((dto, ent) => ent.UserId = dto.UserId ?? userId);
            if (model.Id != null && model.Id != Guid.Empty)
            {
                var application = await _baseLogic.Of<Data.Context.Application>().Base().FirstOrDefaultAsync(a => a.Id == model.Id);
                _ = _baseLogic.Of<Data.Context.Application>().Update(model.MapTo(application, actions));
                return application.Id;
            }
            actions.Add((dto, ent) => ent.Status =  ApplicationStatusEnum.Draft);
            var tmp = await _baseLogic.Of<Data.Context.Application>()
                .Add(model.CreateMappedObject(actions)); 
            await GenerateRegNumber(tmp);

            await _applicationWorkflowLogic.CreateApplication(tmp, userId);
            return tmp;
        }

        public async Task<Guid> InsertOrUpdateRemark(RemarkInDto model, Guid userId)
        {
            var actions = new List<Action<RemarkInDto, Data.Context.Remarks>>();
            actions.Add((dto, ent) => ent.UserId = dto.UserId ?? userId);
            if (model.Id != null && model.Id != Guid.Empty)
            {
                var remarks = await _baseLogic.Of<Data.Context.Remarks>().Base().FirstOrDefaultAsync(a => a.Id == model.Id);
                _ = _baseLogic.Of<Data.Context.Remarks>().Update(model.MapTo(remarks, actions));
                return remarks.Id;
            }
            var tmp = await _baseLogic.Of<Data.Context.Remarks>()
                .Add(model.CreateMappedObject(actions));
            return tmp;
        }

        /// <summary>
        /// Получение информации заявления по id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<object> GetApplication(Guid id)
        {
            var application = await _baseLogic.Of<Data.Context.Application>()
                .Base()
                .AsNoTracking()
                .Select(x => new
                {
                    x.Id,
                    x.Status,
                    x.Number,
                    x.CarriageNumber,
                    x.ContractorsId,
                    x.DefectId,
                    x.RepairPlaceId,
                    x.ReleaseDate,
                    x.FinishDate,
                    x.Description,
                    x.WithReplacement,
                    x.RepairType
                })
                .SingleOrDefaultAsync(x => x.Id == id);

            if (application == null) return null;
            return new { result = application };
        }


        public async Task<object> SetRegNumber(RegNumberInDto model)
        {
            var application = await _baseLogic.Of<Data.Context.Application>().GetById(model.Id);
            application.Number = model.RegNumber;
            return _baseLogic.Of<Data.Context.Application>().Update(application);
        }

        public async Task DeleteApplication(Guid userId, Guid applicationId)
        {
            var application = await _baseLogic.Of<Data.Context.Application>().Base()
                .FirstOrDefaultAsync(a => a.UserId == userId && a.Id == applicationId);
            if (application == default)
                throw new ArgumentException("Ошибка, заявление не найдено");

            await _baseLogic.Of<Data.Context.Application>().Delete(application);
        }

        public async Task DeleteRemark(Guid id)
        {
            var remark = await _baseLogic.Of<Data.Context.Remarks>().Base()
                .FirstOrDefaultAsync(a => a.Id== id);
            if (remark == default)
                throw new ArgumentException("Ошибка, заявление не найдено");

            await _baseLogic.Of<Data.Context.Remarks>().Delete(remark);
        }

        public async Task GenerateRegNumber(Guid applicationId)
        {
            int year = DateTime.Now.Year;
            var result = string.Empty;
            var applicationNumber = await _baseLogic.Of<Data.Context.Application>().Base()
                .Include(x => x.User)
                .Where(a=> a.Number.Substring(6, 4) == year.ToString())?
                .MaxAsync(e => e.Number.Substring(0, 5));

            if (applicationNumber == null)
            {
                result =  "00001_"+ year.ToString();
            }

            if (int.TryParse(applicationNumber, out var numberConvert))
            {
                var newNumber = numberConvert + 1;
                result = newNumber.ToString().PadLeft(5, '0')+"_"+ year.ToString();
            }
            await SetRegNumber(new RegNumberInDto { Id = applicationId, RegNumber = result });
        }

       

        public async Task<object> GetApplications(Guid userId, ApplicationStatusEnum status)
        {
            var currentUserRole = await _baseLogic.Of<Role>().Base()
                .FirstOrDefaultAsync(a => a.UserRoles.Any(t => t.UserId == userId));


            var _ =  _baseLogic.Of<Data.Context.ApplicationTask>().GetQueryable(x => !x.IsDeleted && !x.Application.IsDeleted)
                .AsNoTracking();
            ApplicationStatusEnum applicationStatus = status;
            if (status != ApplicationStatusEnum.All)
            {
                switch (currentUserRole.Value)
                {
                    case RoleEnum.AuditService when status == ApplicationStatusEnum.InWork:
                    case RoleEnum.Economist when status == ApplicationStatusEnum.InWork:
                    case RoleEnum.SalesDepartment when status == ApplicationStatusEnum.InWork:
                    case RoleEnum.TorSpecialist when status == ApplicationStatusEnum.InWork:
                        applicationStatus = ApplicationStatusEnum.InWork;
                        _ = _.Where(a => a.Status == ApplicationStatusEnum.Agreement && a.UserId == userId);
                        break;
                    case RoleEnum.TorManager when status == ApplicationStatusEnum.InWork:
                        applicationStatus = ApplicationStatusEnum.InWork;
                        _ = _.Where(a => a.Status == ApplicationStatusEnum.PaymentFormation && a.UserId == userId);
                        break;
                    case RoleEnum.Treasurer when status == ApplicationStatusEnum.InWork:
                        applicationStatus = ApplicationStatusEnum.InWork;
                        _ = _.Where(a => a.Status == ApplicationStatusEnum.Payment && a.UserId == userId);
                        break;
                    case RoleEnum.Counterparty:
                        _ = _.Where(a => a.Status == status && a.UserId == userId);
                        break;
                    default:
                        _ = _.Where(a => a.Status == status);
                        break;
                }
            }

            var res = _
                .Select(x => new ApplicationOutDto()
                {
                    Id = x.Application.Id,
                    Number = x.Application.Number,
                    DateCreated = x.Application.CreatedDate.ToString("dd.MM.yyyy"),
                    Status = applicationStatus,
                    StatusTitle = GetStatusTitle(x.Status),
                    CarriageNumber = x.Application.CarriageNumber,
                    ContractorsName = x.Application.Contractors.NameRu,
                    DefectName = x.Application.Defect.NameRu,
                    RepairPlaceName = x.Application.RepairPlace.NameRu,
                    IsExpired = x.PlanEndDate != null && DateTime.Now.Subtract((DateTime)x.PlanEndDate).Days > 0,
                    FinishDate = x.Application.FinishDate!=null? ((DateTime)x.Application.FinishDate).ToString("dd.MM.yyyy") :string.Empty,
                    ReleaseDate = x.Application.ReleaseDate.ToString("dd.MM.yyyy")
                })
                .ToList();

            return new
            {
                TotalItems = _.Count(),
                Items = res
            };
        }


        public async Task<object> GetRemarks(Guid applicationId)
        {
            var _ = await _baseLogic.Of<Data.Context.Remarks>().GetQueryable(x => x.ApplicationId == applicationId && !x.IsDeleted)
                .AsNoTracking().Select(a=> new 
                {
                    a.Id,
                    a.ApplicationId,
                    a.Text,
                    CreatedDate = a.CreatedDate.ToString("dd.MM.yyyy"),
                    a.UserId
                }).ToListAsync();


            return _;
        }


        public async Task<object> GetContractorsUsers()
        {

            return _ = await _baseLogic.Of<Data.Context.User>().GetQueryable(x => x.RepairPlaceId != null && !x.IsDeleted)
                .Include(a => a.RepairPlace)
                .AsNoTracking().Select(x => new
                {
                    x.Id,
                    x.RepairPlace.NameRu
                })
                .ToListAsync();
        }



        private static string GetStatusTitle(ApplicationStatusEnum appEnum)
        {

            switch (appEnum)
            {
                case ApplicationStatusEnum.Draft: return "Черновик";
                case ApplicationStatusEnum.InWork: return "В работе";
                case ApplicationStatusEnum.InRepair: return "В ремонте";
                case ApplicationStatusEnum.DocumentCollect: return "Сбор документации";
                case ApplicationStatusEnum.Agreement: return "Согласование";
                case ApplicationStatusEnum.PaymentFormation: return "Формирование заявки на оплату";
                case ApplicationStatusEnum.Payment: return "На оплате";
                case ApplicationStatusEnum.ReWork: return "На доработке";
                case ApplicationStatusEnum.Paid: return "Оплачен";
                default: return "";
            }
        }

        //public async Task SetStatus(Guid applicationId, ApplicationTypeEnum status)
        //{

        //    var application = await _baseLogic.Of<Data.Context.Application>().GetById(applicationId);
        //    application.Status = status;

        //    await _baseLogic.Of<Data.Context.Application>().Update(application);
        //}

        public async Task<object> Statistics(Guid userId)
        {
            var r = await _baseLogic.Of<Data.Context.ApplicationTask>()
                .GetQueryable(x => !x.IsDeleted && !x.Application.IsDeleted)
                .Include(a=>a.Application)
                .AsNoTracking()
                .GroupBy(x => x.Status)
                .Select(x => new
                {
                    x.Key,
                    Count = x.Count()
                })
                .ToListAsync();
            var currentUserRole = await _baseLogic.Of<Role>().Base()
                .FirstOrDefaultAsync(a => a.UserRoles.Any(t => t.UserId == userId));
            var inWorkCount = 0;
            switch (currentUserRole.Value)
            {
                case RoleEnum.AuditService:
                case RoleEnum.Economist:
                case RoleEnum.SalesDepartment:
                case RoleEnum.TorSpecialist:
                    inWorkCount = r.SingleOrDefault(x => x.Key == ApplicationStatusEnum.Agreement)?.Count ?? 0;
                    break;
                case RoleEnum.TorManager:
                    inWorkCount = r.SingleOrDefault(x => x.Key == ApplicationStatusEnum.PaymentFormation)?.Count ?? 0;
                    break;
                case RoleEnum.Treasurer:
                    inWorkCount = r.SingleOrDefault(x => x.Key == ApplicationStatusEnum.Payment)?.Count ?? 0;
                    break;
            }

            return new
            {
                All = r.Sum(x => x.Count),
                Agreement = r.SingleOrDefault(x => x.Key == ApplicationStatusEnum.Agreement)?.Count ?? 0,
                DocumentCollect = r.SingleOrDefault(x => x.Key == ApplicationStatusEnum.DocumentCollect)?.Count ?? 0,
                Draft = r.SingleOrDefault(x => x.Key == ApplicationStatusEnum.Draft)?.Count ?? 0,
                InWork = inWorkCount,
                Paid = r.SingleOrDefault(x => x.Key == ApplicationStatusEnum.Paid)?.Count ?? 0,
                Payment = r.SingleOrDefault(x => x.Key == ApplicationStatusEnum.Payment)?.Count ?? 0,
                PaymentFormation = r.SingleOrDefault(x => x.Key == ApplicationStatusEnum.PaymentFormation)?.Count ?? 0,
                InRepair = r.SingleOrDefault(x => x.Key == ApplicationStatusEnum.InRepair)?.Count ?? 0,
                ReWork = r.SingleOrDefault(x => x.Key == ApplicationStatusEnum.ReWork)?.Count ?? 0
            };
        }


    }
}