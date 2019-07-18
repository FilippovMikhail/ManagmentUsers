using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace WebAPI.Entities
{
    public class User : IdentityUser
    {
        /// <summary>
        /// Имя пользователя
        /// </summary>
        [StringLength(100)]
        [Required]
        public string FirstName { get; set; }
    }
}
