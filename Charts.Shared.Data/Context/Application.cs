using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Charts.Shared.Data.Context.Dictionary;

namespace Charts.Shared.Data.Context
{
    public class Application : BaseEntity
    {
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

        public DateTime ReleaseDate { get; set; }
        public ICollection<Remarks> Remarks { get; private set; } = new HashSet<Remarks>();
    }
}
