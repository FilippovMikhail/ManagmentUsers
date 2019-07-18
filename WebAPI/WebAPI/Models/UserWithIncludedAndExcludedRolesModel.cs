using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WebAPI.Models
{
    public class UserWithIncludedAndExcludedRolesModel
    {
        [Required]
        public string UserName { get; set; }
        [Required]
        public List<string> IncludedRoleNames { get; set; }
        [Required]
        public List<string> ExcludedRoleNames { get; set; }
    }
}
