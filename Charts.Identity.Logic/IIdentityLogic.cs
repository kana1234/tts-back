using Charts.Shared.Data.Context;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace Charts.Identity.Logic
{
    public interface IIdentityLogic
    {
        string GenerateAccessToken(User user);
        string GenerateRefreshToken(User user);
        JwtSecurityToken DecodeToken(string token);
        bool TokenExpired(string token);
    }
}
