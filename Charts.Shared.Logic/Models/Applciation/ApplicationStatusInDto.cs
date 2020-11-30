using System;
using Charts.Shared.Data.Primitives;

namespace Charts.Shared.Logic.Models.Applciation
{
    public class ApplicationStatusInDto
    {
        // Id заявки
        public Guid ApplicationId { get; set; }
        public Guid? UserId { get; set; }
        public string Comment { get; set; }
        // Текущий статус
        public ApplicationTaskStatusEnum Decision { get; set; }
    }
}
