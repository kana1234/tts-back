using System;
using System.Collections.Generic;
using Charts.Shared.Data.Context;
using Charts.Shared.Data.Primitives;

namespace Charts.Shared.Logic.Models.Applciation
{
    public class ApplicationOutDto
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public Data.Context.User User { get; set; }
        public string ClientFullName { get; set; }
        public DateTime DateCreated { get; set; }
        public ApplicationTypeEnum Status { get; set; }
        public string StatusTitle { get; set; }
        public string Number { get; set; }
        public int CarriageNumber { get; set; }
        public Guid? RepairPlaceId { get; set; }
        public Guid? ContractorsId { get; set; }
        public Guid? DefectId { get; set; }
        public List<Remarks> Remarks { get; set; }
    }
}