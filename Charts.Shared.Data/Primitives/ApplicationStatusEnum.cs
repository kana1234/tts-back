namespace Charts.Shared.Data.Primitives
{
    /// <summary>
    /// Статусы
    /// </summary>
    public enum ApplicationStatusEnum
    {
        /// <summary>
        /// Все
        /// </summary>
        All = 1,
        /// <summary>
        /// В ремонте
        /// </summary>
        InWork,
        /// <summary>
        /// Сбор документации 
        /// </summary>
        DocumentCollect,

        /// <summary>
        /// Согласование
        /// </summary>
        Agreement,

        /// <summary>
        /// Формирование заявки на оплату
        /// </summary>
        PaymentFormation,

        /// <summary>
        /// На оплате
        /// </summary>
        Payment,

        /// <summary>
        ///Оплачена
        /// </summary>
        Paid,
        /// <summary>
        /// Черновик
        /// </summary>
        Draft,

        /// <summary>
        /// На доработке
        /// </summary>
        ReWork

    }
}
