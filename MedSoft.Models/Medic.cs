using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Identity;

namespace MedSoft.Models
{
    public class Medic
    {
        [Key]
        public string MedicID { get; set; }
        [ForeignKey("MedicID")]
        [Required]
        [DisplayName("Nume")]
        [MaxLength(50)]
        public string? NumeMedic { get; set; }
        [Required]
        [DisplayName("Prenume")]
        [MaxLength(50)]
        public string? PrenumeMedic { get; set; }
        [Required]
        public long SpecializareID { get; set; }
        [ForeignKey("SpecializareID")]
        [Required]
        [DisplayName("Pret Consultatie")]
        public decimal PretConsultatie { get; set; }
        [Required]
        [MaxLength(50)]
        public string Adresa {  get; set; }
        public string? ImageUrl { get; set; }
        [Required]
        [StringLength(13, MinimumLength = 13, ErrorMessage = "CNP-ul contine 13 cifre.")]
        public string CNP {  get; set; }
        [NotMapped]
        [DisplayName("Specializare")]
        public string? NumeSpecializare { get; set;}
        [ValidateNever]
        public Specializare Specializare { get; set; }
        [NotMapped]
        public string NumeComplet { get => $"{NumeMedic} {PrenumeMedic}"; }
        /*[ValidateNever]
        public IdentityUser AspNetUsers { get; set; }*/
        [NotMapped]
        public string? Email { get; set; }
        [NotMapped]
        public string? NumarTelefon { get; set; }

    }
}
