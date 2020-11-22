using System;
using System.Collections.Generic;
using System.Text;

namespace Charts.Shared.Logic.Models
{
    public class NameIdPair
    {
        public Guid? Id { get; set; }
        public string Name { get; set; }
    }

    public class NameIdCodePair : NameIdPair
    {
        public string Code { get; set; }
    }

}
