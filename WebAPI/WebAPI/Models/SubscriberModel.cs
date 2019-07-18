using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WebAPI.Models
{
    public class SubscriberModel
    {
        /// <summary>
        /// Идентификатор
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// ИНН
        /// </summary>
        [Required]
        public string INN { get; set; }
        /// <summary>
        /// Краткое наименование абонента
        /// </summary>
        [Required]
        public string ShortName { get; set; }
        /// <summary>
        /// Полное наименование абонента
        /// </summary>
        [Required]
        public string FullName { get; set; }
        /// <summary>
        /// Адрес абонента
        /// </summary>
        [Required]
        public string Address { get; set; }
        /// <summary>
        /// Телефоны абонента
        /// </summary>
        [Required]
        public string Phones { get; set; }
        /// <summary>
        /// ФИО руководителя
        /// </summary>
        [Required]
        public string FIOHead { get; set; }
        /// <summary>
        /// Представитель абонента
        /// </summary>
        [Required]
        public string SubscriberRepresentative { get; set; }
        /// <summary>
        /// Телефоны представителя
        /// </summary>
        [Required]
        public string RepresentativePhones { get; set; }
    }
}
