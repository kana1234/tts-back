using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Charts.Shared.Data.Context;
using Charts.Shared.Data.Context.Dictionary;
using Charts.Shared.Data.Primitives;

namespace Charts.Shared.Logic.Models.Applciation
{
    public class RemarkInDto
    {
        public Guid? Id { get; set; }
        [Required]
        public Guid ApplicationId { get; set; }
        public Guid? UserId { get; set; }
        [Required]
        public string Text { get; set; }

    }
}