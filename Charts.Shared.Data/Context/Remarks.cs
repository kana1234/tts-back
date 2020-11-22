using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Charts.Shared.Data.Context
{
    public class Remarks : BaseEntity
    {
        public string NameRu { get; set; }
        public string NameKz { get; set; }

        [ForeignKey(nameof(ApplicationId))]
        public Application Application { get; set; }
        public Guid ApplicationId { get; set; }
    }
}
