using Charts.Shared.Api.AttributeExtension;
using Charts.Shared.Api.Controllers;
using Charts.Shared.Data.Primitives;
using Charts.Shared.Logic.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Charts.Shared.Data.Models;
using Charts.Shared.Logic.Extensions;

namespace Charts.Admin.Api.Controllers
{
    /// <summary>
    /// Контроллер для работы с пользователями
    /// </summary>
    public class UserController : BaseController
    {
        private readonly IUserLogic _logic;
        public UserController(IUserLogic logic)
        {
            _logic = logic;
        }

        /// <summary>
        /// Список ролей и разрешений пользователя
        /// </summary>
        /// <returns></returns>
        [HttpGet("UserRoles")]
        public async Task<IActionResult> UserRoles()
        {
            try
            {
                var _ = await _logic.UserRoles(CurrentUserId);
                return Ok(_);
            }
            catch (Exception e)
            {
                return ExceptionResult(e);
            }
        }

        [HttpPost("AddUser")]
        public async Task<IActionResult> AddUser([FromBody] AdditionRegisterInDto model)
        {
            try
            {
                if (!ModelState.IsValid) return BadRequest(ModelState);
                var _ = await _logic.AddUser(model);
                return Ok(_);
            }
            catch (Exception e)
            {
                return ExceptionResult(e);
            }
        }

        [HttpPost("UpdateUser")]
        public async Task<IActionResult> UpdateUser([FromBody] AdditionShortRegisterInDto model)
        {
            try
            {
                if (!ModelState.IsValid) return BadRequest(ModelState);
                var _ = await _logic.UpdateUser(model);
                return Ok(_);
            }
            catch (Exception e)
            {
                return ExceptionResult(e);
            }
        }

        /// <summary>
        /// Список пользователей
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetAllUsers")]
        [UserRoleAttributeExtension(RoleEnum.Admin)]
        public async Task<IActionResult> GetAllUsers()
        {
            try
            {
                var _ = await _logic.GetAllUsers();
                return Ok(_);
            }
            catch (Exception e)
            {
                return ExceptionResult(e);
            }
        }


        [HttpGet(nameof(GetUser) + "/{login}")]
        public async Task<IActionResult> GetUser(string login)
        {
            try
            {
                if (!ModelState.IsValid) return BadRequest(ModelState);
                var _ = await _logic.GetUser(login);
                return Ok(_);
            }
            catch (Exception e)
            {
                return ExceptionResult(e);
            }
        }

        [HttpGet(nameof(DeleteUser) + "/{login}")]
        public async Task<IActionResult> DeleteUser(string login)
        {
            try
            {
                if (!ModelState.IsValid) return BadRequest(ModelState);
                var _ = await _logic.DeleteUser(login);
                return Ok(_);
            }
            catch (Exception e)
            {
                return ExceptionResult(e);
            }
        }


        /// <summary>
        /// Список ролей
        /// </summary>
        /// <returns></returns>
        [HttpGet("Roles")]
        [UserRoleAttributeExtension(RoleEnum.Admin)]
        public IActionResult Roles()
        {
            try
            {
                var enumVals = new List<object>();
                foreach (var i in Enum.GetValues(typeof(RoleEnum)))
                {
                    enumVals.Add(new
                    {
                        key = i.GetHashCode(),
                        value = (i is RoleEnum ? (RoleEnum) i : (RoleEnum)1).GetDisplayName()
                    });
                }
                return Ok(enumVals);
            }
            catch (Exception e)
            {
                return ExceptionResult(e);
            }
        }
        [HttpGet("Text")]
        [UserRoleAttributeExtension( RoleEnum.Admin)]
        public async Task Text()
        {
          

        }

        [HttpPost("AddRoleToUser")]
        [UserRoleAttributeExtension(RoleEnum.Admin)]
        public async Task AddRoleToUser(Guid UserId, RoleEnum roleEnum)
        {
            try
            {
                var z = await _logic.AddRoleToUser(UserId, roleEnum);
            }
            catch (Exception e)
            {
                throw(e);
            }
                        
        }
    }
}
