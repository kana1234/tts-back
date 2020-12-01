using Charts.Shared.Data.Primitives;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using Charts.Shared.Data.Context.Dictionary;

namespace Charts.Shared.Data.Context
{
    /// <summary>
    /// Пользователи
    /// </summary>
    public class User : BaseEntity
    {
        /// <summary>
        /// Фамилия
        /// </summary>
        public string LastName { get; set; }
        /// <summary>
        /// Имя
        /// </summary>
        public string FirstName { get; set; }
        /// <summary>
        /// Отчество
        /// </summary>
        public string MiddleName { get; set; }

        /// <summary>
        /// Полное имя    
        /// </summary>
        public string FullName => $"{LastName} {FirstName} {MiddleName}".Trim();

        /// <summary>
        /// Дата рождения
        /// </summary>
        public DateTime? BirthDate { get; set; }

        public DateTime? LastInviteDate { get; set; }
        /// <summary>
        /// ИИН/БИН
        /// </summary>
        public string Login { get; set; }
        /// <summary>
		/// Контактный телефон
		/// </summary>
        /// <summary>
        /// Электронный 
        /// </summary>
        public string Email { get; set; }
        /// <summary>
        /// Заблокирован
        /// </summary>
        public bool IsBlocked { get; set; } = false;
        /// <summary>
        /// Зона доступа
        /// </summary>
        public PortalEnum Audience { get; set; }
        /// <summary>
        /// Пароль
        /// </summary>  
        public string Password { get; set; }
        /// <summary>
        /// Кол-во неудачных попыток ввода пароля
        /// </summary>
        public int PasswordTryCount { get; set; } = 0;
        /// <summary>
        /// RefreshToken
        /// </summary>
        public string RefreshToken { get; set; }
        /// <summary>
        /// Список ролей
        /// </summary>
        public virtual ICollection<UserRole> UserRoles { get; private set; } = new HashSet<UserRole>();

        [NotMapped]
        public ICollection<Role> Roles => UserRoles.Select(x => x.Role).ToList();

        [ForeignKey(nameof(ContractorId))]
        public DicContractors Contractors { get; set; }
        public Guid? ContractorId { get; set; }
    }
}
