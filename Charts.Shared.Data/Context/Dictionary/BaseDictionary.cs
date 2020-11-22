namespace Charts.Shared.Data.Context.Dictionary
{
    /// <summary>
    /// Базовый класс для справочников
    /// </summary>
    public class BaseDictionary : BaseEntity
    {
        public string NameRu { get; set; }

        /// <summary>
        /// Наименование на казахском
        /// </summary>
        public string NameKz { get; set; }
    }
}
