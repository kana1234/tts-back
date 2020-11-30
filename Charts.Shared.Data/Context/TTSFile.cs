using System;
using System.ComponentModel.DataAnnotations.Schema;
using Charts.Shared.Data.Primitives;

namespace Charts.Shared.Data.Context
{
    public class TTSFile : BaseEntity
    {
        /// <summary>
        /// Название файла
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Размер файла.
        /// </summary>
        public int Size { get; set; }

        /// <summary>
        /// Справочник: Тип файла
        /// </summary>
        public Guid? FileTypeId { get; set; }

        public Guid? FirstDocTypeId { get; set; }

        /// <summary>
        /// Путь до файла.
        /// </summary>
        public string Path { get; set; }

        /// <summary>
        /// MIME тип файла - контент
        /// </summary>
        public string ContentType { get; set; }

        /// <summary>
        /// Id создавшего сотрудника.
        /// </summary>
        public Guid? CreatorUserId { get; set; }
        public virtual User CreatorUser { get; set; }

        /// <summary>
        /// Дата удаления.
        /// </summary>
        public DateTime? DeleteDateTime { get; set; }

        /// <summary>
        /// Кто удалил.
        /// </summary>
        public Guid? DeletedUserId { get; set; }
        public virtual User DeletedUser { get; set; }

        /// <summary>
        /// Актуален ли файл
        /// К примеру если заключение устарело, замена на новое
        /// </summary>
        public bool IsActual { get; set; }

        /// <summary>
        /// BPM. Информация о загруженном документе.
        /// </summary>
        public Guid? DocInformationId { get; set; }
        [ForeignKey(nameof(DocInformationId))]
        public DocInformation DocInformation { get; set; }
        public FileType FileType { get; set; }
        /// <summary>
        /// Id заявки к которой принадлежит файл.
        /// </summary>
        public Guid? ApplicationId { get; set; }
        public virtual Application Application { get; set; }

    }
}
