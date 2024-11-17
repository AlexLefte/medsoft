using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedSoft.Models
{
    public class Consultatie
    {
        [Key]
        public long ConsultatieID { get; set; }
        [Required]
        [MaxLength(50)]
        public DateTime Data { get; set; }
        [MaxLength(50)]
        public string? Diagnostic { get; set; }
        [MaxLength(50)]
        public string? Doza { get; set; }
        [Required]
        [MaxLength(50)]
        public string Status { get; set;}
        
        [Required]  
        public string PacientID { get; set; }
        [ForeignKey("PacientID")]
        [ValidateNever]
        public Pacient Pacient { get; set; }      
        public long? MedicamentID { get; set; }
        [ForeignKey("MedicamentID")]
        [ValidateNever]
        public Medicamente Medicamente { get; set; }
        [Required]
        public string MedicID {  get; set; }
        [ForeignKey("MedicID")]
        public Medic Medic { get; set; }

    }
}
