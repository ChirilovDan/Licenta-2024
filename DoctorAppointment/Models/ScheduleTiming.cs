#nullable disable
using System.ComponentModel.DataAnnotations;

namespace DoctorAppointment.Models
{
    public class ScheduleTiming
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [Display(Name ="Select Day")]
        public string Day { get; set; }
        [Required]
        [Display(Name = "Add Slot")]
        public string Slot { get; set; }    
        public string userId { get; set; }  
        public DateTime CurrentYear {  get; set; }  
    }
}
