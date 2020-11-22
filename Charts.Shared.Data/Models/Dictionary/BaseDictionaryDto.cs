using System;
using System.ComponentModel.DataAnnotations;

namespace Charts.Shared.Data.Models.Dictionary
{
    public class BaseDictionaryDto
    {
        public Guid? Id { get; set; }
        [Required]
        public string NameRu { get; set; }
        [Required]
        public string NameKz { get; set; }
    }
}
