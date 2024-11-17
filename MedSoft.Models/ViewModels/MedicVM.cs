using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace MedSoft.Models.ViewModels
{
    public class MedicVM
    {
        /// <summary>
        /// Medic
        /// </summary>
        [ValidateNever]
        public Medic Medic { get; set; }

        /// <summary>
        /// User
        /// </summary>
        [ValidateNever]
        public IdentityUser User { get; set; }

        /*/// <summary>
        /// Parola
        /// </summary>
        [ValidateNever]
        [DataType(DataType.Password)]
        [Display(Name="Parola")]
        [StringLength(100, ErrorMessage = "{0} trebuie sa contina cel putin {2} si maximum {1} caractere.", MinimumLength = 6)]
        public string? Password { get; set; }*/

        /// <summary>
        /// Parola noua
        /// </summary>
        [DataType(DataType.Password)]
        [StringLength(100, ErrorMessage = "{0} trebuie sa contina cel putin {2} si maximum {1} caractere.", MinimumLength = 6)]
        [Display(Name = "Parola")]
        [RequireWhenNew]
        public string? NewPassword { get; set; }

        /// <summary>
        /// Confirmare parola noua
        /// </summary>
        [DataType(DataType.Password)]
        [RequireWhenNew]
        [Compare("NewPassword", ErrorMessage = "Parolele nu corespund.")]
        public string? ConfirmNewPassword { get; set; }

        /// <summary>
        /// Email
        /// </summary>
        [Required(ErrorMessage = "Camp obligatoriu")]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        /// <summary>
        /// Numar de telefon
        /// </summary>
        [Required(ErrorMessage = "Camp obligatoriu")]
        [DataType(DataType.PhoneNumber)]
        [Display(Name = "Numar de telefon")]
        [RegularExpression(@"^(\+\d{1,2}\s?)?\(?\d{3}\)?[\s.-]?\d{3}[\s.-]?\d{4}$", ErrorMessage = "Numar de telefon invalid")]
        public string NumarTelefon { get; set; }

        /// <summary>
        /// Specializare
        /// </summary>
        [DataType(DataType.Text)]
        // [ValidateNever]
        public int? Specializare { get; set; }

        /// <summary>
        /// Lista specializari
        /// </summary>
        [ValidateNever]
        public List<SelectListItem> ListaSpecializari { get; set; }

        public class RequireWhenNewAttribute : ValidationAttribute
        {
            protected override ValidationResult IsValid(object value, ValidationContext validationContext)
            {
                var medic = (MedicVM)validationContext.ObjectInstance;
                if (medic.Medic.MedicID == null && value == null)
                {
                    return new ValidationResult("Camp obligatoriu.");                    
                }
                return ValidationResult.Success;
            }
        }
    }
}
