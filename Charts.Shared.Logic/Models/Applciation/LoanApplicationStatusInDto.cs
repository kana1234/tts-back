using System;
using Charts.Shared.Data.Primitives;

namespace Charts.Shared.Logic.Models.Applciation
{
    public class LoanApplicationStatusInDto
    {
        // Id заявки
        public Guid ApplicationId { get; set; }

        // Текущий статус
        public ApplicationTypeEnum Status { get; set; }
    }
}
