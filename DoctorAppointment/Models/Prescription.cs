#nullable disable
using System.ComponentModel.DataAnnotations;

namespace DoctorAppointment.Models
{
    public class Prescription
    {
        [Key]
        public int id {  get; set; }
        [Required]
        public string Name { get; set; }   
        public DateTime PrescriptionDate { get; set; }
        public string PatientId { get; set; }
    }
}
