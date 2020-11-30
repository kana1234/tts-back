using System;
using System.Collections.Generic;
using Charts.Shared.Data.Primitives;

namespace Charts.Shared.Logic.Models.File
{
    public class FileInDto
    {
        public Guid Id { get; set; }
        public Guid? AppId { get; set; }
        public Guid? BasePledgeId { get; set; }
        public int Size { get; set; }
        public string Type { get; set; }
        public string Code { get; set; }
        public FileType Page { get; set; }
        public string Title { get; set; }
        public DateTime? Date { get; set; }
        public string Number { get; set; }
        public IEnumerable<Guid> FileIds { get; set; }

        public string PageInterval { get; set; }
        public int PageCount { get; set; }
        public bool IsOriginal { get; set; }
    }
}
