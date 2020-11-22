using Charts.Identity.Logic.Models;
using Charts.Shared.Data.Context;
using Charts.Shared.Data.Primitives;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Charts.Shared.Logic;
using Microsoft.EntityFrameworkCore;

namespace Charts.Identity.Logic
{
    public class AccountLogic : IAccountLogic
    {
        private readonly IBaseLogic _baseLogic;
        private readonly IIdentityLogic _identityLogic;


        public AccountLogic(IBaseLogic baseLogic, IIdentityLogic identityLogic)
        {
            _baseLogic = baseLogic;
            _identityLogic = identityLogic;
        }

        public async Task<object> Login(LoginInDto dto)
        {
            var user = await _baseLogic.Base<User>()
                .GetQueryable(x => x.Login == dto.Login && !x.IsDeleted)
                .Include(x => x.UserRoles)
                .ThenInclude(x => x.Role)
                .FirstOrDefaultAsync();

            if (user == default)
                throw new ArgumentException("Пользователь не найден");

            if (user.IsBlocked)
                throw new ArgumentException("Пользователь заблокирован, обратитесь к администратору");

            var pwdhash = HashPwd(dto.Password);
            if (user.Password != pwdhash)
            {
                user.PasswordTryCount++;
                if (user.PasswordTryCount >= 5)
                    user.IsBlocked = true;
                await _baseLogic.Base<User>().Update(user);
                throw new ArgumentException("Вами введен некорректный пароль. Введите пароль еще раз");
            }

            // сброс пароля
            user.PasswordTryCount = 0;
            user.LastInviteDate = DateTime.Now;


            var accessToken = _identityLogic.GenerateAccessToken(user);
            var refreshToken = _identityLogic.GenerateRefreshToken(user);

            user.RefreshToken = refreshToken;
            await _baseLogic.Base<User>().Update(user);
            return new
            {
                accessToken,
                refreshToken
            };
        }

        public async Task<object> UpdateToken(string jwtToken, string refToken)
        {
            var decoded = _identityLogic.DecodeToken(jwtToken);
            if (decoded == default)
                throw new UnauthorizedAccessException();

            var value = decoded.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Sub)?.Value;
            if (value == default || !Guid.TryParse(value, out Guid userId))
                throw new UnauthorizedAccessException();

            var user = await _baseLogic.Base<User>().GetQueryable(x => x.Id == userId)
                .Include(x => x.UserRoles)
                .ThenInclude(x => x.Role)
                .FirstOrDefaultAsync();
            if (user == default)
                throw new ArgumentException("Пользователь не найден");

            if (_identityLogic.TokenExpired(refToken) || user.RefreshToken != refToken)
                throw new UnauthorizedAccessException();
            var accessToken = _identityLogic.GenerateAccessToken(user);
            var refreshToken = _identityLogic.GenerateRefreshToken(user);
            user.RefreshToken = refreshToken;
            await _baseLogic.Base<User>().Update(user);
            return new
            {
                accessToken,
                refreshToken
            };
        }

        public async Task<Guid> Register(AdditionRegisterInDto model)
        {
            var _ = model as AdditionRegisterInDto;
            var result = await _baseLogic.Base<User>().GetQueryable(x => x.Login == _.Login)
                .AsNoTracking()
                .FirstOrDefaultAsync();
            if (result != default)
                throw new ArgumentException("Данный пользователь уже зарегистрирован в системе");

            var user = new User
            {
                Login = _.Login,
                LastName = _.LastName,
                FirstName = _.FirstName,
                MiddleName = _.MiddleName,
                Password = HashPwd(_.Password),
                Audience = PortalEnum.Ext
            };
            var role = await _baseLogic.Base<Role>().Base().FirstOrDefaultAsync(a => a.Value == model.RoleId);
            user.UserRoles.Add(new UserRole
            {
                RoleId = role.Id
            });
            return await _baseLogic.Base<User>().Add(user);
        }

      

        public async Task<bool> GetByIdentifier(string login)
        {
            var _ = await _baseLogic.Base<User>().GetQueryable(x => x.Login == login && !x.IsDeleted).CountAsync();
            return _ > 0;
        }

        private string HashPwd(string pwd)
        {
            var alg = SHA512.Create();
            alg.ComputeHash(Encoding.UTF8.GetBytes(pwd));
            return Convert.ToBase64String(alg.Hash);
        }


    }
}
