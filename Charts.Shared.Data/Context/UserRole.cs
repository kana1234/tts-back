using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Charts.Shared.Data.Context
{
    [Table("UserRoles")]
    public class UserRole : BaseEntity
    {
        [ForeignKey(nameof(UserId))]
        public User User { get; set; }
        public Guid UserId { get; set; }


        [ForeignKey(nameof(RoleId))]
        public Role Role { get; set; }
        public Guid RoleId { get; set; }
    }
}