using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Charts.Shared.Data.Context;
using Charts.Shared.Data.Context.Dictionary;
using Charts.Shared.Data.Primitives;

namespace Charts.Shared.Logic.Models.Applciation
{
    public class ApplicationInDto
    {
        public Guid? Id { get; set; }
        public Guid? UserId { get; set; }
        public string Number { get; set; }
        public int CarriageNumber { get; set; }
        public Guid? RepairPlaceId { get; set; }

        public Guid? ContractorsId { get; set; }
        public ApplicationTypeEnum Status { get; set; }
        public Guid? DefectId { get; set; }

        [Required]
        public DateTime ReleaseDate { get; set; }
        public List<Remarks> Remarks { get; set; }
    }
}