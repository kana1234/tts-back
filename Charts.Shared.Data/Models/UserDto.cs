using System;
using System.Collections.Generic;
using System.Text;
using Charts.Shared.Data.Context;
using Charts.Shared.Data.Primitives;

namespace Charts.Shared.Data.Models
{
    public class UserDto
    {
        public string FullName { get; set; }
        public string Login { get; set; }

        public string Email { get; set; }
        public bool IsActive { get; set; }
        public string ActiveString { get; set; }
        public string LastInviteDate { get; set; }
        public string RolesString { get; set; }
        /// <summary>
        /// Фамилия
        /// </summary>
        public string LastName { get; set; }
        /// <summary>
        /// Имя
        /// </summary>
        public string FirstName { get; set; }
        /// <summary>
        /// Отчество
        /// </summary>
        public string MiddleName { get; set; }
        public Guid? ContractorId { get; set; }
        public RoleEnum? RoleId { get; set; }
        public IEnumerable<Role> Roles { get; set; }
    }
}
