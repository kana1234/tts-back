using System;
using System.ComponentModel.DataAnnotations;

namespace Charts.Shared.Logic.Models.Applciation
{
    public class LoanApplicationInDto
    {
        public Guid ApplicationId { get; set; }

        public Guid UserId { get; set; }
    }
}
