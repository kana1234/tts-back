using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Charts.Logic;
using Charts.Shared.Api.Controllers;
using Charts.Shared.Data.Context;
using Charts.Shared.Data.Context.Dictionary;
using Charts.Shared.Data.Primitives;
using Charts.Shared.Logic;
using Charts.Shared.Logic.Dictionary;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Charts.Api.Controllers
{
    [AllowAnonymous]
    public class ChartsController : BaseController
    {
        private readonly IBaseLogic _baseLogic;
        private readonly IChartLogic _chartLogic;
      
        public ChartsController(IBaseLogic baseLogic, IChartLogic chartLogic)
        {
            _baseLogic = baseLogic;
            _chartLogic = chartLogic;
        }

        
        [HttpGet(nameof(GetExpiredContractors))]
        public async Task<IActionResult> GetExpiredContractors()
        {
            try
            {
                var userIds = await _baseLogic.Of<UserRole>().GetQueryable(a => a.Role.Value == RoleEnum.Counterparty)
                    .Include(a=>a.Role)
                    .Select(a => a.UserId).ToListAsync();

                var tmpAll = await _baseLogic.Of<ApplicationTask>()
                    .GetQueryable(a => userIds.Any(t=>t.Equals(a.UserId)))
                    .GroupBy(a=>a.ApplicationId).Select(a=>new
                    {
                        a.Key,
                        Count = a.Count()
                    }).ToListAsync();

                var tmpExpired = await _baseLogic.Of<ApplicationTask>()
                    .GetQueryable(a => userIds.Any(t => t == a.UserId) && a.PlanEndDate > DateTime.Now)
                    .GroupBy(a => a.ApplicationId).Select(a => new
                    {
                        a.Key,
                        Count = a.Count()
                    }).ToListAsync();
                var data = await _baseLogic.Of<DicContractors>().Base().ToListAsync();

               var result = data.AsEnumerable().Select(a =>
                    new
                    {
                        Name = a.NameRu, 
                        All = tmpAll.Where(t => t.Key == a.Id).SingleOrDefault().Count,
                        Expired = tmpExpired.Where(t => t.Key == a.Id).SingleOrDefault().Count
                    }
                );
                return Ok(result);
            }
            catch (Exception e)
            {
                return ExceptionResult(e);
            }
        }
    }
}
