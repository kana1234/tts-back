using System;
using Charts.Shared.Data.Primitives;

namespace Charts.Shared.Logic.Models.File
{
    public class FileUploadEntry
    {
        public string Identifier { get; set; }
        public Guid? AppicationId { get; set; }
        public FileType FileType { get; set; }
    }
}
