using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Camunda.Api.Client;
using Camunda.Api.Client.UserTask;
using Charts.Shared.Data.Extensions;
using Charts.Shared.Data.Primitives;
using Charts.Shared.Logic.Dictionary;
using Charts.Shared.Logic.Helper;
using Charts.Shared.Logic.Models.Applciation;
using Charts.Shared.Logic.User;
using Microsoft.EntityFrameworkCore;

namespace Charts.Shared.Logic.Application
{
    public class ApplicationLogic : IApplicationLogic
    {

        private readonly IDictionaryLogic _dictionarylogic;
        private readonly IUserLogic _userLogic;

        private readonly IBaseLogic _baseLogic;
        private IApplicationLogic _loanApplicationLogicImplementation;

        public ApplicationLogic(IDictionaryLogic dictionarylogic, IUserLogic userLogic, IBaseLogic baseLogic, IApplicationLogic loanApplicationLogicImplementation)
        {
            _dictionarylogic = dictionarylogic;
            _userLogic = userLogic;
            _baseLogic = baseLogic;
            _loanApplicationLogicImplementation = loanApplicationLogicImplementation;
        }

        public async Task<Guid> InsertOrUpdateApplication(ApplicationInDto model,Guid userId)
        {
            var actions = new List<Action<ApplicationInDto, Data.Context.Application>>();
            actions.Add((dto, ent) => ent.UserId = dto.UserId ?? userId);
            if (model.Id != null && model.Id != Guid.Empty)
            {
                var application = await _baseLogic.Of<Data.Context.Application>().Base().FirstOrDefaultAsync(a => a.Id == model.Id);
                _ = _baseLogic.Of<Data.Context.Application>().Update(model.MapTo(application, actions));
                return application.Id;
            }
            
            var tmp = await _baseLogic.Of<Data.Context.Application>()
                .Add(model.CreateMappedObject(actions)); 
            await GenerateRegNumber(tmp);
            return tmp;
        }


        public async Task<object> GetApplicationById(Guid id)
        {
            return await _baseLogic.Of<Data.Context.Application>().GetById(id);
        }

        public async Task<object> SetRegNumber(RegNumberInDto model)
        {
            var application = await _baseLogic.Of<Data.Context.Application>().GetById(model.Id);
            application.Number = model.RegNumber;
            return _baseLogic.Of<Data.Context.Application>().Update(application);
        }

        public async Task GenerateRegNumber(Guid applicationId)
        {
            int year = DateTime.Now.Year;
            var result = string.Empty;
            var applicationNumber = await _baseLogic.Of<Data.Context.Application>().GetQueryable(x => x.Id == applicationId)
                .Include(x => x.User)
                .Where(a=> a.Number.Substring(8, 4) == year.ToString())
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

        /// <summary>
        /// Gets Applications by filter
        /// </summary>
        /// <param name="filter"></param>
        /// <returns>TotalItems, Items: List<LoanApplicationOutDto></returns>
        public virtual object GetApplications(LoanApplicationFilter filter)
        {

            var query = _baseLogic.Of<Data.Context.Application>().GetQueryable
            (x => !x.IsDeleted).AsNoTracking();

            var search = !string.IsNullOrEmpty(filter.Search) ? filter.Search.ToLower().Trim() : null;
            if (search != null)
                query = query
                    .Include(x => x.User)
                    .Include(x => x.Remarks)
                    .Where(x =>
                    (x.User.LastName + " " + x.User.FirstName + " " + x.User.MiddleName).ToLower().Trim().Contains(search)
                     || x.User.Login.ToLower().Trim().Contains(search));

            var res = query
                    .OrderBy(filter.Column, nameof(Data.Context.Application.Id), filter.Direction)
                    .Skip(filter.Skip)
                    .Take(filter.PageSize)
                    .Select(x => new ApplicationOutDto()
                    {
                        Id = x.Id,
                        UserId = x.UserId,
                        ClientFullName = x.User.FullName,
                        Number = x.Number,
                        DateCreated = x.CreatedDate,
                        Status = x.Status,
                        StatusTitle = GetStatusTitle(x.Status),
                        CarriageNumber =x.CarriageNumber,
                        ContractorsId = x.ContractorsId,
                        DefectId = x.DefectId,
                        RepairPlaceId = x.RepairPlaceId,
                        User = x.User,
                        Remarks = x.Remarks.ToList()
                    })
                    .ToList();

            return new
            {
                TotalItems = query.Count(),
                Items = res
            };
        }


        /// <summary>
        /// Gets Applications By UserId
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public object GetApplications(LoanApplicationFilter filter, Guid userId)
        {
            var query = _baseLogic.Of<Data.Context.Application>().GetQueryable
            (x => x.UserId == userId && !x.IsDeleted).AsNoTracking();

            var res = query
                    .OrderBy(filter.Column, nameof(Data.Context.Application.Id), filter.Direction)
                    .Skip(filter.Skip)
                    .Take(filter.PageSize)
                    .Select(x => new ApplicationOutDto()
                    {
                        Id = x.Id,
                        UserId = x.UserId,
                        ClientFullName = x.User.FullName,
                        Number = x.Number,
                        DateCreated = x.CreatedDate,
                        Status = x.Status,
                        StatusTitle = GetStatusTitle(x.Status),
                        CarriageNumber = x.CarriageNumber,
                        ContractorsId = x.ContractorsId,
                        DefectId = x.DefectId,
                        RepairPlaceId = x.RepairPlaceId,
                        User = x.User

                    })
                    .ToList();

            return new
            {
                TotalItems = query.Count(),
                Items = res
            };
        }

        public async Task<object> GetApplications(Guid userId)
        {
            var query =  _baseLogic.Of<Data.Context.Application>().GetQueryable
                (x => x.UserId == userId && !x.IsDeleted).AsNoTracking();

            var res = query
                .Select(x => new ApplicationOutDto()
                {
                    Id = x.Id,
                    UserId = x.UserId,
                    ClientFullName = x.User.FullName,
                    Number = x.Number,
                    DateCreated = x.CreatedDate,
                    Status = x.Status,
                    StatusTitle = GetStatusTitle(x.Status),
                    CarriageNumber = x.CarriageNumber,
                    ContractorsId = x.ContractorsId,
                    DefectId = x.DefectId,
                    RepairPlaceId = x.RepairPlaceId,
                    User = x.User

                })
                .ToList();

            return new
            {
                TotalItems = query.Count(),
                Items = res
            };
        }



        private static string GetStatusTitle(ApplicationTypeEnum appEnum)
        {

            switch (appEnum)
            {
                case ApplicationTypeEnum.NEW: return "Новая заявка";
                case ApplicationTypeEnum.IN_WORK: return "В работе";
                case ApplicationTypeEnum.REMARK: return "На доработке";
                case ApplicationTypeEnum.FINISHED: return "Выполнен";
                default: return "";
            }
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
                .Include(x => x.User)
                .AsNoTracking()
                .Select(x => new
                {
                    x.Id,
                    x.UserId,
                    Status = x.Status,
                    x.Number,
                    x.CarriageNumber,
                    x.ContractorsId,
                    x.DefectId,
                    x.RepairPlaceId
                })
                .SingleOrDefaultAsync(x => x.Id == id);

            if (application == null) return null;

            

            return new
            {

            };
        }

        public async Task SetStatus(Guid applicationId, ApplicationTypeEnum status)
        {

            var application = await _baseLogic.Of<Data.Context.Application>().GetById(applicationId);
            application.Status = status;

            await _baseLogic.Of<Data.Context.Application>().Update(application);
        }

        public async Task<object> Statistics()
        {
            var r = await _baseLogic.Of<Data.Context.Application>()
                .Base()
                .Where(x => !x.IsDeleted)
                .AsNoTracking()
                .GroupBy(x => x.Status)
                .Select(x => new
                {
                    x.Key,
                    Count = x.Count()
                })
                .ToListAsync();


            return new
            {
                All = r.Sum(x => x.Count),
                //Enbek_CMNew = r.SingleOrDefault(x => x.Key == ApplicationTypeEnum.Enbek_CMNew)?.Count ?? 0,
                //Enbek_CMReview = r.SingleOrDefault(x => x.Key == ApplicationTypeEnum.Enbek_CMReview)?.Count ?? 0,
                //Enbek_CMInWork = r.SingleOrDefault(x => x.Key == ApplicationTypeEnum.Enbek_CMInWork)?.Count ?? 0,
                //Enbek_CMRework = r.SingleOrDefault(x => x.Key == ApplicationTypeEnum.Enbek_CMRework)?.Count ?? 0,
                //Enbek_CmAll = r
                //    .Where(x => x.Key >= ApplicationTypeEnum.Enbek_CMNew && x.Key <= ApplicationTypeEnum.Enbek_CMArchive
                //    || x.Key == ApplicationTypeEnum.Enbek_CMFinished)
                //    .Sum(x => x.Count),
                //Enbek_PrepareCreditDossier = r.SingleOrDefault(x => x.Key == ApplicationTypeEnum.Enbek_PrepareCreditDossier)?.Count ?? 0,
                //Enbek_Completed = r.SingleOrDefault(x => x.Key == ApplicationTypeEnum.Enbek_Completed)?.Count ?? 0,
                //Enbek_CMArchive = r.SingleOrDefault(x => x.Key == ApplicationTypeEnum.Enbek_CMArchive)?.Count ?? 0,

                //Enbek_URBoss = r.SingleOrDefault(x => x.Key == ApplicationTypeEnum.Enbek_URBoss)?.Count ?? 0,
                //Enbek_EPledge = r.SingleOrDefault(x => x.Key == ApplicationTypeEnum.Enbek_EPledge)?.Count ?? 0,
                //Enbek_EAct = r.SingleOrDefault(x => x.Key == ApplicationTypeEnum.Enbek_EAct)?.Count ?? 0,
                //Enbek_URNew = r.SingleOrDefault(x => x.Key == ApplicationTypeEnum.Enbek_URNew)?.Count ?? 0,
                //Enbek_URRework = r.SingleOrDefault(x => x.Key == ApplicationTypeEnum.Enbek_URRework)?.Count ?? 0,
                //Enbek_UrAll = r
                //    .Where(x => x.Key >= ApplicationTypeEnum.Enbek_URNew && x.Key <= ApplicationTypeEnum.Enbek_URRework)
                //    .Sum(x => x.Count),
                //Enbek_CCNew = r.SingleOrDefault(x => x.Key == ApplicationTypeEnum.Enbek_CCNew)?.Count ?? 0,
            };
        }
    }
}