using Charts.Identity.Logic;
using Charts.Identity.Logic.Models;
using Charts.Shared.Api.Controllers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace Charts.Identity.Api.Controllers
{
    /// <summary>
    /// Контроллер авторизации
    /// </summary>

    [AllowAnonymous]
    public class AccountController : BaseController
    {
        private readonly IAccountLogic _logic;
        public AccountController(IAccountLogic logic)
        {
            _logic = logic;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginInDto login)
        {
            try
            {
                if (!ModelState.IsValid) return BadRequest(ModelState);
                var _ = await _logic.Login(login);
                return Ok(_);
            }
            catch (Exception e)
            {
                return ExceptionResult(e);
            }
        }

        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh()
        {
            try
            {
                var _ = await _logic.UpdateToken(
                    HttpContext.Request.Headers["X-Access-Token"].ToString(),
                    HttpContext.Request.Headers["X-Refresh-Token"].ToString()
                );
                return Ok(_);
            }
            catch (UnauthorizedAccessException)
            {
                return StatusCode(403);
            }
            catch (Exception e)
            {
                return ExceptionResult(e);
            }
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] AdditionRegisterInDto model)
        {
            try
            {
                if (!ModelState.IsValid) return BadRequest(ModelState);
                var _ = await _logic.Register(model);
                return Ok(_);
            }
            catch (Exception e)
            {
                return ExceptionResult(e);
            }
        }

      

        [HttpGet("{login}")]
        public async Task<IActionResult> GetByIdentifier([FromRoute][Required] string login)
        {
            try
            {
                if (!ModelState.IsValid) return BadRequest(ModelState);
                var _ = await _logic.GetByIdentifier(login);
                return Ok(_);
            }
            catch (Exception e)
            {
                return ExceptionResult(e);
            }
        }
    }
}
