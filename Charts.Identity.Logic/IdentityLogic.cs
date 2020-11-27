using Charts.Shared.Data;
using Charts.Shared.Data.Context;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using Newtonsoft.Json;

namespace Charts.Identity.Logic
{
    public class IdentityLogic : IIdentityLogic
    {
        private readonly IOptions<AppSettings> _conf;
        public IdentityLogic(IOptions<AppSettings> conf)
        {
            _conf = conf;
        }

        public string GenerateAccessToken(User user)
        {
            var signCred = new SigningCredentials(new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_conf.Value.AuthOptions.Key)), SecurityAlgorithms.HmacSha256);
            var now = DateTime.UtcNow;
            var jwt = new JwtSecurityToken(
                issuer: _conf.Value.AuthOptions.Issuer,
                audience: _conf.Value.AuthOptions.Audience,
                notBefore: now,
                claims: GenerateToken(user).Claims,
                expires: now.Add(TimeSpan.FromMinutes(_conf.Value.AuthOptions.Lifetime)),
                signingCredentials: signCred);
            var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);
            return encodedJwt;
        }

        public string GenerateRefreshToken(User user)
        {
            var signCred = new SigningCredentials(new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_conf.Value.AuthOptions.Key)), SecurityAlgorithms.HmacSha256);
            var now = DateTime.UtcNow;
            var jwt = new JwtSecurityToken(
                issuer: _conf.Value.AuthOptions.Issuer,
                audience: _conf.Value.AuthOptions.Audience,
                notBefore: now,
                claims: GenerateToken(user).Claims,
                expires: now.Add(TimeSpan.FromMinutes(_conf.Value.AuthOptions.LifetimeRefresh)),
                signingCredentials: signCred);
            var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);
            return encodedJwt;
        }

        public bool TokenExpired(string token)
        {
            var decoded = DecodeToken(token);
            if (decoded == null) return true;

            var expires = decoded.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Exp);
            if (expires == null || !double.TryParse(expires.Value, out double timestamp))
                return true;

            var expireDate = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc).AddSeconds(timestamp);

            return DateTime.UtcNow > expireDate;
        }

        private ClaimsIdentity GenerateToken(User user)
        {
            try
            {
                var claims = new List<Claim>
                {
                    new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                    new Claim(JwtRegisteredClaimNames.Aud, user.Audience.ToString()),
                    new Claim("login", user.Login),
                    new Claim("lastName", user.LastName??string.Empty),
                    new Claim("firstName", user.FirstName??string.Empty),
                    new Claim("middleName", user.MiddleName??string.Empty),
                    new Claim(ClaimsIdentity.DefaultRoleClaimType, string.Join(",", user.Roles.Select(x => (int)x.Value))),
                    new Claim("roles", JsonConvert.SerializeObject(user.Roles.Select(role => (int) role.Value).ToArray())??string.Empty)
                };
                ClaimsIdentity claimsIdentity =
                    new ClaimsIdentity(claims, "Token", ClaimsIdentity.DefaultNameClaimType, ClaimsIdentity.DefaultRoleClaimType);
                return claimsIdentity;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
           
        }

        public JwtSecurityToken DecodeToken(string token)
        {
            var handler = new JwtSecurityTokenHandler();
            return handler.CanReadToken(token) ? handler.ReadJwtToken(token) : null;
        }
    }
}
