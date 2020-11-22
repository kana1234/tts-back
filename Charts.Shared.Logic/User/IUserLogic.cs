using Charts.Shared.Data.Context;
using Charts.Shared.Data.Primitives;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Charts.Shared.Data.Models;

namespace Charts.Shared.Logic.User
{
    public interface IUserLogic
    {
        Task<object> UserRoles(Guid userId);
        Task<object> AddRoleToUser(Guid userId, RoleEnum roleEnum);
        Task<List<UserRole>> GetUsersByRoleBranch(RoleEnum roleEnum, Guid BranchId);
        Task<object> GetAllUsers();
        Task<bool> AddUser(AdditionRegisterInDto model);
        Task<bool> UpdateUser(AdditionShortRegisterInDto model);
        Task<AdditionRegisterInDto> GetUser(string login);
        Task<bool> DeleteUser(string login);
    }
}
