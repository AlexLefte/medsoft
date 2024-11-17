using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace MedSoft.Models
{
    public class Pacient
    {
        [Key]
        public string PacientID { get; set; }
        [Required]
        [StringLength(13, MinimumLength = 13, ErrorMessage = "CNP-ul contine 13 cifre.")]
        public string? CNP { get; set; }
        [Required]
        [DisplayName("Nume")]
        [MaxLength(45)]
        public string? NumePacient { get; set; }
        [Required]
        [DisplayName("Prenume")]
        [MaxLength(45)]
        public string? PrenumePacient { get; set; }
        [Required]
        [MaxLength(45)]
        public string? Adresa { get; set; }
        [Required]
        [MaxLength(45)]
        public decimal? Asigurare { get; set; }
        [NotMapped]
        public string NumeComplet { get => $"{NumePacient} {PrenumePacient}"; } 

    }
}