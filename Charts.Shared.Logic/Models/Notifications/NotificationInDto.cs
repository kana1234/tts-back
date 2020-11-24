using System;
using Charts.Shared.Data.Primitives;

namespace Charts.Shared.Logic.Models.Notifications
{
    public class NotificationInDto
    {
        public Guid ApplicationId { get; set; }
        public string SubjectKz { get; set; }
        public string SubjectRu { get; set; }
        public string BodyKz { get; set; }
        public string BodyRu { get; set; }
        public NotificationStatusEnum StatusCode { get; set; }
        public string Type { get; set; }
        public string TaskCode { get; set; }
        public bool IsRead { get; set; }
        public string Error { get; set; }
    }
}
