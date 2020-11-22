using Charts.Shared.Data.Primitives;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;

namespace Charts.Shared.Data.Context
{
    [Table("Roles")]
    public class Role : BaseEntity
    {
        public RoleEnum Value { get; set; }
        public ICollection<UserRole> UserRoles { get; private set; } = new HashSet<UserRole>();
    }
}
