using System;
using System.ComponentModel.DataAnnotations.Schema;
using Charts.Shared.Data.Primitives;

namespace Charts.Shared.Data.Context
{
    public class Notification: BaseEntity
    {
        public Guid ApplicationId { get; set; }
        [ForeignKey(nameof(ApplicationId))]
        public Application LoanApplication { get; set; }

        public string TaskCode { get; set; }
        public string SubjectKz { get; set; }
        public string SubjectRu { get; set; }
        public string BodyKz { get; set; }
        public string BodyRu { get; set; }
        public string Error { get; set; }
        public NotificationStatusEnum  StatusCode { get; set; }


        
        public bool IsRead { get; set; }
    }
}
