using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Charts.Logic.Models
{
    public class GaugeChartDto
    {
        public string Title { get; set; }
        public decimal Data { get; set; }
        public int Size { get; set; }
        public int MaxTemp { get; set; }
        public int MinTemp { get; set; }
    }
}
