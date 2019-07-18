using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WebAPI.Models
{
    public class RoleViewModel
    {
        /// <summary>
        /// Идентификатор
        /// </summary>
        public string Id { get; set; }
        /// <summary>
        /// Название роли на русском
        /// </summary>
        public string RoleName { get; set; }
        /// <summary>
        /// Отображаемое название роли
        /// </summary>
        public string DisplayName { get; set; }
        /// <summary>
        /// Разрешено добавлять
        /// </summary>
        public bool CanCreate { get; set; }
        /// <summary>
        /// Разрешено изменять
        /// </summary>
        public bool CanEdit { get; set; }
        /// <summary>
        /// Разрешено просматривать
        /// </summary>
        public bool CanShow { get; set; }
        /// <summary>
        /// Разрешено печатать
        /// </summary>
        public bool CanPrint { get; set; }
    }
}
