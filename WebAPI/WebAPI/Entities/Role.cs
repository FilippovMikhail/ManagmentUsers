using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WebAPI.Entities
{
    public class Role : IdentityRole
    {
        /// <summary>
        /// Отображаемое название роли (на русском языке)
        /// </summary>
        [Required]
        [StringLength(100)]
        public string DisplayName { get; set; }
        /// <summary>
        /// Разрешено добавлять
        /// </summary>
        [Required]
        public bool CanCreate { get; set; }
        /// <summary>
        /// Разрешено изменять
        /// </summary>
        [Required]
        public bool CanEdit { get; set; }
        /// <summary>
        /// Разрешено просматривать
        /// </summary>
        [Required]
        public bool CanShow { get; set; }
        /// <summary>
        /// Разрешено печатать
        /// </summary>
        [Required]
        public bool CanPrint { get; set; }
    }
}
