using Charts.Identity.Logic.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Charts.Identity.Logic
{
    public interface IAccountLogic
    {
        Task<object> Login(LoginInDto dto);
        Task<object> UpdateToken(string accessToken, string refToken);
        Task<Guid> Register(AdditionRegisterInDto model);
        Task<bool> GetByIdentifier(string identifier);
    }
}
