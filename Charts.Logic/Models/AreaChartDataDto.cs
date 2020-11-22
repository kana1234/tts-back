using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Charts.Logic.Models
{
    public class AreaChartDataDto
    {
        public string Title { get; set; }
        public string SubTitle { get; set; }
        public string [] LabelList { get; set; }
        public double [] DataList { get; set; }
        public int Size { get; set; }
    }
}
