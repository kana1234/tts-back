using Charts.Shared.Data.Primitives;
using Microsoft.AspNetCore.Authorization;
using System.Linq;

namespace Charts.Shared.Api.AttributeExtension
{
    /// <summary>
    /// Расширение для 
    /// </summary>
    public class UserRoleAttributeExtension : AuthorizeAttribute
    {
        public UserRoleAttributeExtension(params RoleEnum[] roles)
        {
            Roles = string.Join(",", roles.Select(x => (int)x));
        }
    }
}
