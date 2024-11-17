using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace MedSoft.Models
{
    public class Administrator
    {
        [Key]
        public string AdministratorID { get; set; }
        [Required]
        [MaxLength(50)]
        public string Nume {  get; set; }
        [Required]
        [MaxLength(50)]
        public string Prenume { get; set; }
        [Required]
        [StringLength(13)]
        public string CNP { get; set; }
        [Required]
        [MaxLength(50)]
        public string Adresa {  get; set; }
    }
}
