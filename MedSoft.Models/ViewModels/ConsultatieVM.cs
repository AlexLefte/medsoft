using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace MedSoft.Models.ViewModels
{
    public class ConsultatieVM
    {
        /// <summary>
        /// Consultatie
        /// </summary>
        [ValidateNever]
        public Consultatie Consultatie { get; set; }

        [DataType(DataType.Date)]
        [Required]
        public DateTime Data { get; set; }

        [Required]
        public string Ora { get; set; }

        /// <summary>
        /// Colectie specializari
        /// </summary>
        [ValidateNever]
        public IEnumerable<SelectListItem> ListaSpecializari { get; set; }

        /// <summary>
        /// ID-ul specilizarii dorite
        /// Pe baza lui, sunt afisati medicii corespunzatori respectivei specializari.
        /// </summary>
        [ValidateNever]
        public int SpecializareID { get; set; }

        /// <summary>
        /// Lista medici
        /// </summary>
        [ValidateNever]
        public IEnumerable<SelectListItem> ListaMedici { get; set; }

        /// <summary>
        /// Lista pacienti
        /// </summary>
        [ValidateNever]
        public IEnumerable<SelectListItem> ListaPacienti { get; set; }

        /// <summary>
        /// Lista medicamente
        /// </summary>
        [ValidateNever]
        public IEnumerable<SelectListItem> ListaMedicamente { get; set; }

        /// <summary>
        /// Lista intervale orare
        /// </summary>
        [ValidateNever]
        public IEnumerable<SelectListItem> ListaIntervaleOrare { get; set; }
    }
}
