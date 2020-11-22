using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Charts.Shared.Data.Primitives;

namespace Charts.Logic.Models
{
    public class ChartDataShellDto
    {
        public List<AreaChartDataDto> AreaCharts { get; set; }
        public GaugeChartDto GaugeChart { get; set; }
        public ChartTypeEnum ChartType { get; set; }
    }
}
