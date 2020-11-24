namespace Charts.Shared.Data.Context
{
    public class NotificationTemplate:BaseEntity
    {
        public string TaskCode { get; set; }
        public string SubjectKz { get; set; }
        public string SubjectRu { get; set; }
        public string BodyKz { get; set; }
        public string BodyRu { get; set; }

    }
}
