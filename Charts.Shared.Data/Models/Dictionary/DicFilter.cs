using System.Configuration;

namespace Charts.Shared.Data.Models.Dictionary
{
    public class DicFilter
    {
        [IntegerValidator(MinValue = 0)]
        public int PageIndex { get; set; } = 0;
        [IntegerValidator(MinValue = 1, MaxValue = 1000, ExcludeRange = true)]
        public int PageSize { get; set; } = 20;
        public int Skip => PageIndex * PageSize;
        public string Search { get; set; }
        public string Column { get; set; }
        public string Direction { get; set; }
    }
}
