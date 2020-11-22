﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using Charts.Shared.Data.Primitives;

namespace Charts.Identity.Logic.Models
{
    public class AdditionRegisterInDto
    {
        [Required]
        public string Login { get; set; }
        public string LastName { get; set; }
        [Required]
        public string FirstName { get; set; }
        public string MiddleName { get; set; }

        [Required, MinLength(8)]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        [Required]
        public RoleEnum RoleId { get; set; }
    }
}
