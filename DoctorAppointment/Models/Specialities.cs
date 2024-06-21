#nullable disable
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DoctorAppointment.Models
{
    public class Specialities
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [Display(Name = "Specialities Name")]
        public string Name { get; set; }
        [Required]
        [Display(Name = "Specialities Image")]
        public string Image {  get; set; }
        [NotMapped]
        public IFormFile ProductFile { get; set; }  

    }
}
