using System;
using System.ComponentModel.DataAnnotations.Schema;
using Charts.Shared.Data.Primitives;

namespace Charts.Shared.Data.Context
{
    public class ApplicationHistory : BaseEntity
    {
        /// <summary>
        /// Идентификатор заявки
        /// </summary>
        [ForeignKey(nameof(ApplicationId))]
        public Application Application { get; set; }
        public Guid ApplicationId { get; set; }

        /// <summary>
        /// Идентификатор статуса
        /// </summary>
        public ApplicationTaskStatusEnum Status { get; set; }

        /// <summary>
        /// Идентификатор роли
        /// </summary>
        [ForeignKey(nameof(RoleId))]
        public Role Role { get; set; }
        public Guid? RoleId { get; set; }

        /// <summary>
        /// Идентификатор пользователя
        /// </summary>
        [ForeignKey(nameof(UserId))]
        public User User { get; set; }
        public Guid? UserId { get; set; }

        /// <summary>
        ///  Дата назначения пользователю
        /// </summary>
        public DateTime? AppointmentDate { get; set; }

        /// <summary>
        ///  Дата планового завершения
        /// </summary>
        public DateTime? PlanEndDate { get; set; }

        /// <summary>
        ///  Дата фактического завершения
        /// </summary>
        public DateTime? FactEndDate { get; set; }

        /// <summary>
        ///  Просрочки дней
        /// </summary>
        public int? ExpireDays => FactEndDate?.Subtract(PlanEndDate ?? FactEndDate.Value).Days;

        /// <summary>
        ///  Результат решения
        /// </summary>
        //[ForeignKey(nameof(DecisionId))]
        //public DicDecision DicDecision { get; set; }
        //public Guid? DecisionId { get; set; }

        /// <summary>
        /// Комментарии
        /// </summary>
        public string Comment { get; set; }
    }
}
