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
    public class PacientVM
    {
        /// <summary>
        /// Pacient
        /// </summary>
        [ValidateNever]
        public Pacient Pacient { get; set; }

        /// <summary>
        /// User
        /// </summary>
        [ValidateNever]
        public IdentityUser User { get; set; }

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
        /// Verificarea validitatii parolei
        /// </summary>
        public class RequireWhenNewAttribute : ValidationAttribute
        {
            protected override ValidationResult IsValid(object value, ValidationContext validationContext)
            {
                var pacient = (PacientVM)validationContext.ObjectInstance;
                if (pacient.Pacient.PacientID == null && value == null)
                {
                    return new ValidationResult("Camp obligatoriu.");                    
                }
                return ValidationResult.Success;
            }
        }
    }
}
