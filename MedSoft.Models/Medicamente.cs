using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MedSoft.Models
{
    public class Medicamente
    {
        [Key]
        public long MedicamentID { get; set; }
        [Required]
        [MaxLength(45)]
        public string Denumire { get; set; }
        public string? ImageUrl { get; set; }
    }
}
