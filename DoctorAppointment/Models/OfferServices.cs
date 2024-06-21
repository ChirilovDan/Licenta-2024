#nullable disable
using System.ComponentModel.DataAnnotations;

namespace DoctorAppointment.Models
{
    public class OfferServices
    {
        [Key]
        public int ID { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public decimal Price { get; set; }
        public string DoctorId { get; set; }
    }
}
