using System;
using System.Threading.Tasks;
using Charts.Logic.Application;
using Charts.Shared.Api.Controllers;
using Charts.Shared.Logic.Models.Applciation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Charts.Api.Controllers
{
    /// <summary>
    /// Контроллер авторизации
    /// </summary>

    [AllowAnonymous]
    public class ApplicationController : BaseController
    {
        private readonly IApplicationLogic _applicationLogic;

        public ApplicationController(IApplicationLogic applicationLogic)
        {
            _applicationLogic = applicationLogic;
        }
        /// <summary>
        /// Получение списка заявлений
        /// </summary>
        /// <returns></returns>
        [HttpPost(nameof(GetApplications))]
        public IActionResult GetApplications()
        {
            try
            {
                var res = _applicationLogic.GetApplications(CurrentUserId);
                return Ok(res);
            }
            catch (Exception e)
            {
                return ExceptionResult(e);
            }
        }

        [HttpPost(nameof(InsertOrUpdateApplication))]
        public IActionResult InsertOrUpdateApplication([FromBody] ApplicationInDto model)
        {
            try
            {
                if (!ModelState.IsValid) return BadRequest(ModelState);
                var res = _applicationLogic.InsertOrUpdateApplication(model,CurrentUserId);
                return Ok(res);
            }
            catch (Exception e)
            {
                return ExceptionResult(e);
            }
        }

        /// <summary>
        /// Статистика кол-во заявок
        /// </summary>
        /// <returns></returns>
        //[HttpGet(nameof(Statistics))]
        //public async Task<IActionResult> Statistics()
        //{
        //    try
        //    {
        //        var res = await _applicationLogic.StatisticsTasks(CurrentUserId);
        //        return Ok(res);
        //    }
        //    catch (Exception e)
        //    {
        //        return ExceptionResult(e);
        //    }
        //}

        [HttpPost("SetStatus")]
        public async Task<IActionResult> SetStatus(LoanApplicationStatusInDto model)
        {
            try
            {
                await _applicationLogic.SetStatus(model.ApplicationId, model.Status);

                return Ok(true);
            }
            catch (Exception e)
            {
                return ExceptionResult(e);
            }
        }

        [HttpGet("GetClientApllications")]
        public IActionResult GetClientApllications([FromQuery] LoanApplicationFilter filter)
        {
            try
            {
                if (filter.PageSize < 1) return NoContent();

                var res = _applicationLogic.GetApplications(filter, CurrentUserId);

                return Ok(res);
            }
            catch (Exception e)
            {
                return ExceptionResult(e);
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetApplication(Guid id)
        {
            try
            {
                var res = await _applicationLogic.GetApplication(id);

                return Ok(res);
            }
            catch (Exception e)
            {
                return ExceptionResult(e);
            }
        }

    }
}
