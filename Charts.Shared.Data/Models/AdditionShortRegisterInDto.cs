using System;
using System.ComponentModel.DataAnnotations;
using Charts.Shared.Data.Primitives;

namespace Charts.Shared.Data.Models
{
    public class AdditionShortRegisterInDto
    {
        public string LastName { get; set; }
        [Required]
        public string FirstName { get; set; }
        public string Login { get; set; }
        public string MiddleName { get; set; }

        [Required]
        public RoleEnum? RoleId { get; set; }
        public string Email { get; set; }
        public Guid? ContractorId { get; set; }
    }
}
