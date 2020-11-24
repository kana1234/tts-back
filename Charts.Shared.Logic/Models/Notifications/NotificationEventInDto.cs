using System;
using Charts.Shared.Data.Primitives;

namespace Charts.Shared.Logic.Models.Notifications
{
    public class NotificationEventInDto
    {
        public string TaskCode { get; set; }
        public Guid LoanApplicationId { get; set; }
        public string CommentRu { get; set; }
        public string CommentKz { get; set; }
        public string CommentRu2 { get; set; }
        public string CommentKz2 { get; set; }
        public string Error { get; set; }
        public NotificationStatusEnum StatusCode { get; set; }
    }
}
