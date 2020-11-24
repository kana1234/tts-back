using System;

namespace Charts.Shared.Logic.Models.Applciation
{
    public class LoanApplicationFilter
    {
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public int Skip => PageIndex * PageSize;
        public string Search { get; set; }
        public string Column { get; set; }
        public string Direction { get; set; }
        public Guid? ApplicationId { get; set; }
    }
}
