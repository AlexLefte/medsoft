using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace MedSoft.Models
{
    public class Specializare
    {
        [Key]
        public long SpecializareID { get; set; }
        [Required]
        [DisplayName("Nume Specializare")]
        [MaxLength(30)]
        public string? Nume { get; set; }
        [NotMapped]
        public int? NumarMedici {  get; set; }
    }
}
