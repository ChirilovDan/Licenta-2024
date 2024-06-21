#nullable disable
using System.ComponentModel.DataAnnotations;

namespace DoctorAppointment.Models
{
    public class CardDetails
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [Display(Name ="Card Holder Name")]
        public string Name { get; set; }
        [Required]
        public string CardNumber { get; set; }
        [Required]
        public DateTime ExpiryDate {  get; set; }
        [Required]
        public string CVV { get; set; } 
        public int OrderID {  get; set; }   
    }
}
