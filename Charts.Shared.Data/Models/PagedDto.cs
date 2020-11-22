using System;
using System.Collections.Generic;
using System.Text;

namespace Charts.Shared.Data.Models
{
    public class PagedDto<BaseEntity>
    {
        public int CurrentPage { get; set; }
        public int PageCount { get; set; }
        public int PageSize { get; set; }
        public int RowCount { get; set; }
        public IList<BaseEntity> Results { get; set; }
    }
}
