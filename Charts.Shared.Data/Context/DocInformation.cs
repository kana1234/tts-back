using System;
using System.ComponentModel.DataAnnotations;
using Charts.Shared.Data.Primitives;

namespace Charts.Shared.Data.Context
{
    public class DocInformation : BaseEntity
    {
        /// <summary>
        /// Номер документа
        /// </summary>
        public string Number { get; set; }

        /// <summary>
        /// Дата выдачи документа
        /// </summary>
        public DateTime? Date { get; set; }

        /// <summary>
        /// Оригинал/Копия
        /// </summary>
        public bool IsOriginal { get; set; }



        /// <summary>
        /// Тип документа, для 1С
        /// </summary>
        public string DocumentCode { get; set; }

        /// <summary>
        /// Количество страниц
        /// </summary>
        public int PageCount { get; set; } = 1;

        [MaxLength(50)]
        public string PageInterval { get; set; }

        public string Title { get; set; }
        public FileType FileType { get; set; }
    }
}
