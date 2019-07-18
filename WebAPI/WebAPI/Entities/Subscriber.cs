using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace WebAPI.Entities
{
    public class Subscriber
    {
        /// <summary>
        /// Идентификатор
        /// </summary>
        [Required]
        public int Id { get; set; }
        /// <summary>
        /// ИНН
        /// </summary>
        [Required]
        [Column(TypeName = "nvarchar(12)")]
        public string INN { get; set; }
        /// <summary>
        /// Краткое наименование абонента
        /// </summary>
        [Required]
        [Column(TypeName = "nvarchar(50)")]
        public string ShortName { get; set; }
        /// <summary>
        /// Полное наименование абонента
        /// </summary>
        [Required]
        [Column(TypeName = "nvarchar(4000)")]
        public string FullName { get; set; }
        /// <summary>
        /// Адрес абонента
        /// </summary>
        [Required]
        [Column(TypeName = "nvarchar(200)")]
        public string Address { get; set; }
        /// <summary>
        /// Телефоны абонента
        /// </summary>
        [Required]
        [Column(TypeName = "nvarchar(200)")]
        public string Phones { get; set; }
        /// <summary>
        /// ФИО руководителя
        /// </summary>
        [Required]
        [Column(TypeName = "nvarchar(200)")]
        public string FIOHead { get; set; }
        /// <summary>
        /// Представитель абонента
        /// </summary>
        [Required]
        [Column(TypeName = "nvarchar(4000)")]
        public string SubscriberRepresentative { get; set; }
        /// <summary>
        /// Телефоны представителя
        /// </summary>
        [Required]
        [Column(TypeName = "nvarchar(200)")]
        public string RepresentativePhones { get; set; }
    }
}
