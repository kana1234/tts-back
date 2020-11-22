using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;

namespace Charts.Shared.Api.Controllers
{
    /// <summary>
    /// Базовый контроллер
    /// </summary>
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class BaseController : ControllerBase
    {
        private Guid _userId = new Guid();

        protected Guid CurrentUserId
        {
            get
            {
                if (_userId != default) return _userId;

                var claimValue = User
                    .Claims
                    .FirstOrDefault(x => x.Properties.Values.Contains(JwtRegisteredClaimNames.Sub))?
                    .Value;
                if (claimValue == null) return _userId;
                _userId = Guid.Parse(claimValue);

                return _userId;
            }
        }

        protected IActionResult ExceptionResult(Exception ex, object args = null)
        {
            var controllerName = ControllerContext.ActionDescriptor.ControllerName;
            var actionName = ControllerContext.ActionDescriptor.ActionName;
#if DEBUG
            var msg = $"{controllerName} {actionName} {ex.Message}";
#else
            var msg = $"{ex.Message}";
#endif
            var detailMsg = $"{ex.Message}, Trace: {ex.StackTrace}";
            switch (ex)
            {
                case Exception e when e is ArgumentException:
                    Log.Warning(ex, msg, args);
                    return BadRequest(msg);

                case Exception e when e is UnauthorizedAccessException:
                    Log.Warning(ex, msg, args);
                    return StatusCode(StatusCodes.Status401Unauthorized, ex.Message);
#if DEBUG
                default:
                    Log.Error(ex, msg, args);
                    return StatusCode(StatusCodes.Status500InternalServerError, detailMsg);
#else
                default:
                    Log.Error(ex, msg, args);
                    return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
#endif
            }
        }
    }
}
