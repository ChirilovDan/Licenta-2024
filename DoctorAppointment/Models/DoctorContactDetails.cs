#nullable disable
using System.ComponentModel.DataAnnotations;

namespace DoctorAppointment.Models
{
    public class DoctorContactDetails
    {
        [Key]
        public int id {  get; set; }
        [Required]
        [Display(Name ="Adress")]
        public string Adress { get; set; }
        [Required]
        [Display(Name = "City")]
        public string City { get; set; }
        [Required]
        [Display(Name = "State")]
        public string State { get; set; }
        [Required]
        [Display(Name = "Country")]
        public string Country { get; set; }
        [Required]
        [Display(Name = "Postal Code")]
        public string PostalCode { get; set; }
        [Required]
        [Display(Name = "Doctor Fee")]
        public decimal DoctorFee { get; set; }
        [Required]
        [Display(Name = "Doctor availability time ")]
        public DateTime DoctorAvailability { get; set; }    

        public string UserID {  get; set; } 
    }
}
