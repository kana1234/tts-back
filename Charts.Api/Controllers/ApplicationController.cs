using System;
using System.Threading.Tasks;
using Charts.Shared.Api.Controllers;
using Charts.Shared.Data.Primitives;
using Charts.Shared.Logic.Application;
using Charts.Shared.Logic.Models.Applciation;
using Charts.Shared.Logic.Workflow.Application;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Charts.Api.Controllers
{
    /// <summary>
    /// Контроллер авторизации
    /// </summary>

    public class ApplicationController : BaseController
    {
        private readonly Shared.Logic.Application.IApplicationLogic _applicationLogic;
        private readonly IApplicationWorkflowLogic _applicationWorkflowLogic;


        public ApplicationController(IApplicationLogic applicationLogic, IApplicationWorkflowLogic applicationWorkflowLogic)
        {
            _applicationLogic = applicationLogic;
            _applicationWorkflowLogic = applicationWorkflowLogic;
        }
        /// <summary>
        /// Получение списка заявлений
        /// </summary>
        /// <returns></returns>
        [HttpGet(nameof(GetApplications) + "/{status}")]
        public async Task<IActionResult> GetApplications(ApplicationStatusEnum status)
        {
            try
            {
                var res = await _applicationLogic.GetApplications(CurrentUserId, status);
                return Ok(res);
            }
            catch (Exception e)
            {
                return ExceptionResult(e);
            }
        }

      

        [HttpGet(nameof(GetContractorsUsers))]
        public async Task<IActionResult> GetContractorsUsers()
        {
            try
            {
                var res = await _applicationLogic.GetContractorsUsers();
                return Ok(res);
            }
            catch (Exception e)
            {
                return ExceptionResult(e);
            }
        }


        [HttpPost(nameof(SendToUser))]
        public async Task<IActionResult> SendToUser([FromBody] ApplicationStatusInDto model)
        {
            try
            {
                await _applicationWorkflowLogic.SendApplicationToUser(model, CurrentUserId);
                return Ok();
            }
            catch (Exception e)
            {
                return ExceptionResult(e);
            }
        }



        [HttpGet(nameof(DeleteApplication) + "/{applicationId}")]
        public async Task<IActionResult> DeleteApplication(Guid applicationId)
        {
            try
            {
                await _applicationLogic.DeleteApplication(CurrentUserId, applicationId);
                return NoContent();
            }
            catch (Exception e)
            {
                return ExceptionResult(e);
            }
        }

        [HttpPost(nameof(InsertOrUpdateApplication))]
        public async Task<IActionResult> InsertOrUpdateApplication([FromBody] ApplicationInDto model)
        {
            try
            {
                if (!ModelState.IsValid) return BadRequest(ModelState);
                var res = await _applicationLogic.InsertOrUpdateApplication(model,CurrentUserId);
               
               
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
        [HttpGet(nameof(Statistics))]
        public async Task<IActionResult> Statistics()
        {
            try
            {
                var res = await _applicationLogic.Statistics(CurrentUserId);
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


        [HttpGet(nameof(GetRemarks) + "/{applicationId}")]
        public async Task<IActionResult> GetRemarks(Guid applicationId)
        {
            try
            {
                var res = await _applicationLogic.GetRemarks(applicationId);
                return Ok(res);
            }
            catch (Exception e)
            {
                return ExceptionResult(e);
            }
        }

        [HttpGet(nameof(DeleteRemark) + "/{id}")]
        public async Task<IActionResult> DeleteRemark(Guid id)
        {
            try
            {
                await _applicationLogic.DeleteRemark(id);
                return NoContent();
            }
            catch (Exception e)
            {
                return ExceptionResult(e);
            }
        }

        [HttpPost(nameof(InsertOrUpdateRemark))]
        public async Task<IActionResult> InsertOrUpdateRemark([FromBody] RemarkInDto model)
        {
            try
            {
                if (!ModelState.IsValid) return BadRequest(ModelState);
                var res = await _applicationLogic.InsertOrUpdateRemark(model, CurrentUserId);


                return Ok(res);
            }
            catch (Exception e)
            {
                return ExceptionResult(e);
            }
        }

    }
}
