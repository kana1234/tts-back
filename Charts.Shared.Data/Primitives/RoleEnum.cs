using System.ComponentModel.DataAnnotations;

namespace Charts.Shared.Data.Primitives
{
    /// <summary>
    /// Роли
    /// </summary>
    public enum RoleEnum
    {
        /// <summary>
        /// Администратор
        /// </summary>
        [Display(Name = "Администратор")]
        Admin = 1,
        /// <summary>
        /// Тор специалист
        /// </summary>
        [Display(Name = "Тор специалист")]
        TorSpecialist,
        /// <summary>
        /// Экономист
        /// </summary>
        [Display(Name = "Экономист")]
        Economist,
        /// <summary>
        /// Отдел перв.док.
        /// </summary>
        [Display(Name = "Отдел перв.док.")]
        SalesDepartment,
        /// <summary>
        /// ревизиональная служба
        /// </summary>
        [Display(Name = "Ревизиональная служба")]
        AuditService,

        /// <summary>
        /// Контрагент
        /// </summary>
        [Display(Name = "Контрагент")]
        Counterparty,

        /// <summary>
        /// Менеджер отдела тор
        /// </summary>
        [Display(Name = "Менеджер отдела тор")]
        TorManager,

        /// <summary>
        /// Казначей
        /// </summary>
        [Display(Name = "Казначей")]
        Treasurer

    }
}
