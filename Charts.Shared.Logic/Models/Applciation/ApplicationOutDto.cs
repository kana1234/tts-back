using System;
using System.Collections.Generic;
using Charts.Shared.Data.Context;
using Charts.Shared.Data.Primitives;

namespace Charts.Shared.Logic.Models.Applciation
{
    public class ApplicationOutDto
    {
        public Guid Id { get; set; }
        public string ClientFullName { get; set; }
        public bool IsExpired { get; set; }
        public string DateCreated { get; set; }
        public ApplicationStatusEnum Status { get; set; }
        public string StatusTitle { get; set; }
        public string Number { get; set; }
        public int CarriageNumber { get; set; }
        public string RepairPlaceName { get; set; }
        public string ContractorsName { get; set; }
        public string DefectName { get; set; }
        public string ReleaseDate { get; set; }
        public string FinishDate { get; set; }
    }
}