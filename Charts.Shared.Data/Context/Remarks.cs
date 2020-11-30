using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Charts.Shared.Data.Context
{
    public class Remarks : BaseEntity
    {
        public string Text { get; set; }
        public Guid UserId { get; set; }

        [ForeignKey(nameof(ApplicationId))]
        public Application Application { get; set; }
        public Guid ApplicationId { get; set; }
    }
}
