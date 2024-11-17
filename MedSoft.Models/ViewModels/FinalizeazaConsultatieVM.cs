using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedSoft.Models.ViewModels
{
    public class FinalizeazaConsultatieVM
    {
        /// <summary>
        /// Consultatie
        /// </summary>
        [ValidateNever]
        public Consultatie Consultatie { get; set; }

        [MaxLength(50)]
        [Required(ErrorMessage = "Câmp obligatoriu.")]
        public string? Diagnostic { get; set; }

        [MaxLength(50)]
        [Required(ErrorMessage = "Câmp obligatoriu.")]
        public string? Doza { get; set; }

        [Required(ErrorMessage = "Câmp obligatoriu.")]
        public long MedicamentID { get; set; }
    }
}
