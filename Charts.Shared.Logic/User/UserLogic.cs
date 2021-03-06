﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Charts.Shared.Data.Context;
using Charts.Shared.Data.Models;
using Charts.Shared.Data.Primitives;
using Charts.Shared.Logic.Extensions;
using Microsoft.EntityFrameworkCore;

namespace Charts.Shared.Logic.User
{
    public class UserLogic : IUserLogic
    {

        private readonly DataContext _context;
        private readonly IBaseLogic _baseLogic;


        public UserLogic(DataContext context, IBaseLogic baseLogic)
        {
            _context = context;
            _baseLogic = baseLogic;
        }
        public async Task<object> UserRoles(Guid userId)
        {
            var _ = await _baseLogic.Of<UserRole>().GetQueryable(x => !x.IsDeleted && x.UserId == userId)
                .AsNoTracking()
                .Include(x => x.Role)
                .Select(x => new
                {
                    x.Id,
                    x.Role.Value
                })
                .ToListAsync();
            return _;
        }


        public async Task<object> GetAllUsers()
        {
            var _ = await _baseLogic.Of<Data.Context.User>().GetQueryable(x => !x.IsDeleted)
                .AsNoTracking()
                .Include(x => x.UserRoles).ThenInclude(a=>a.Role)
                .Include(a=>a.Contractors)
                .Select(a=>new UserDto
                    {
                        IsActive = !a.IsBlocked,
                        ActiveString = a.IsBlocked?"Нет":"Да",
                        FullName = a.FullName,
                        Email = a.Email,
                        LastInviteDate = a.LastInviteDate.HasValue?a.LastInviteDate.Value.ToShortDateString():DateTime.Now.ToShortDateString(),
                        Login = a.Login,
                        Roles = a.UserRoles.Select(t=>t.Role),
                        FirstName =a.FirstName,
                        LastName = a.LastName,
                        MiddleName = a.MiddleName,
                        ContractorId = a.ContractorId,
                        RoleId = a.UserRoles.FirstOrDefault().Role.Value
                }
                    )
                .ToListAsync();
            foreach (var item in _)
            {
                var tempRoles = item.Roles.Distinct().Select(a => a.Value.GetDisplayName())
                    .ToList();
                item.RolesString = tempRoles.Aggregate((c, n) => c + ", " + n);
            }
            return _;
        }

        public async Task<AdditionRegisterInDto> GetUser(string login)
        {
            var result = await _baseLogic.Of<Data.Context.User>().GetQueryable(x => x.Login == login && !x.IsDeleted)
                .AsNoTracking()
                .FirstOrDefaultAsync();

            return new AdditionRegisterInDto
            {
                FirstName = result.FirstName,
                LastName = result.LastName,
                RoleId = result.Roles.FirstOrDefault()?.Value,
                ContractorId = result.ContractorId,
                Email = result.Email,
                Login = result.Login,
                MiddleName = result.MiddleName,
            };
        }

        public async Task<bool> DeleteUser(string login)
        {
            try
            {
                var result = await _baseLogic.Of<Data.Context.User>().GetQueryable(x => x.Login == login)
                    .AsNoTracking()
                    .FirstOrDefaultAsync();
                if (result != null)
                {
                    result.IsDeleted = true;
                    await _baseLogic.Of<Data.Context.User>().Update(result);
                }

                return true;

            }
            catch (Exception e)
            {
                return false;
            }
        }


        public async Task<bool> AddUser(AdditionRegisterInDto model)
        {
            var _ = model as AdditionRegisterInDto;
            var result = await _baseLogic.Of<Data.Context.User>().GetQueryable(x => x.Login == _.Login)
                .AsNoTracking()
                .FirstOrDefaultAsync();
            var role = await _baseLogic.Of<Role>().Base().FirstOrDefaultAsync(a => a.Value == model.RoleId);
            if (result != default)
            {
                return false;
            }
            else
            {
                var user = new Data.Context.User
                {
                    Login = _.Login,
                    Email = _.Email,
                    LastName = _.LastName,
                    FirstName = _.FirstName,
                    MiddleName = _.MiddleName,
                    Password = HashPwd(_.Password),
                    Audience = PortalEnum.Int,
                    ContractorId = _.ContractorId
                };

                user.UserRoles.Add(new UserRole
                {
                    RoleId = role.Id
                });
                await _baseLogic.Of<Data.Context.User>().Add(user);
                return true;
            }

           
        }

        public async Task<bool> UpdateUser(AdditionShortRegisterInDto model)
        {
            var _ = model as AdditionShortRegisterInDto;
            var result = await _baseLogic.Of<Data.Context.User>().Base()
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Login == _.Login);
            var role = await _baseLogic.Of<Role>().Base().FirstOrDefaultAsync(a => a.Value == model.RoleId);
            if (result != default)
            {
                result.LastName = _.LastName;
                result.FirstName = _.FirstName;
                result.MiddleName = _.MiddleName;
                result.ContractorId = _.ContractorId;
                result.Email = _.Email;
                //result.UserRoles.FirstOrDefault().RoleId = role.Id;
                await _baseLogic.Of<Data.Context.User>().Update(result);
                var userRole = await _baseLogic.Of<Data.Context.UserRole>().Base()
                    .FirstOrDefaultAsync(a => a.UserId == result.Id);
                userRole.RoleId = role.Id;
                await _baseLogic.Of<Data.Context.UserRole>().Update(userRole);
                return true;
            }
            else
            {
                return false;
            }


        }


        private string HashPwd(string pwd)
        {
            var alg = SHA512.Create();
            alg.ComputeHash(Encoding.UTF8.GetBytes(pwd));
            return Convert.ToBase64String(alg.Hash);
        }

        public async Task<object> AddRoleToUser(Guid userId, RoleEnum roleEnum)
        {
            var role = await _baseLogic.Of<Role>().GetQueryable(x => x.Value == roleEnum).FirstAsync();
            UserRole userRole = new UserRole();
            userRole.UserId = userId;
            userRole.RoleId = role.Id;
            await _baseLogic.Of<UserRole>().Add(userRole);
            return true;
        }

        public async Task<List<UserRole>> GetUsersByRoleBranch(RoleEnum roleEnum, Guid BranchId)
        {
            var _ = await _baseLogic.Of<UserRole>().GetQueryable(x => !x.IsDeleted)
                .Include(x => x.Role)
                .Include(x => x.User)
                .Where(x => x.Role.Value == roleEnum  && !x.User.IsDeleted)
                .ToListAsync();
            return _;
        }

    }
}
