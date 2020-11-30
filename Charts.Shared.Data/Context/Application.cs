using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Charts.Shared.Data.Context.Dictionary;
using Charts.Shared.Data.Primitives;

namespace Charts.Shared.Data.Context
{
    public class Application : BaseEntity
    {
        public Guid UserId { get; set; }
        [ForeignKey(nameof(UserId))]
        public User User { get; set; }

        public Guid? ContragentUserId { get; set; }
        [ForeignKey(nameof(ContragentUserId))]
        public User ContragentUser { get; set; }

        public string Number { get; set; }
        public int CarriageNumber { get; set; }

        [ForeignKey(nameof(RepairPlaceId))]
        public DicRepairPlace RepairPlace { get; set; }
        public Guid? RepairPlaceId { get; set; }

        [ForeignKey(nameof(ContractorsId))]
        public DicContractors Contractors { get; set; }
        public Guid? ContractorsId { get; set; }


        [ForeignKey(nameof(DefectId))]
        public DicDefect Defect { get; set; }
        public Guid? DefectId { get; set; }
        public ApplicationStatusEnum Status { get; set; }
        public DateTime ReleaseDate { get; set; }
        public DateTime? FinishDate { get; set; }
        public string Description { get; set; }
        public bool WithReplacement { get; set; } = false;
        public RepairType RepairType { get; set; }
        public string InstanceId { get; set; }
        public ICollection<Remarks> Remarks { get; private set; } = new HashSet<Remarks>();
    }
}
