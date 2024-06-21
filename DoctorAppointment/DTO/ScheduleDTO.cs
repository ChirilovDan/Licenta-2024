#nullable disable
using System.ComponentModel.DataAnnotations;

namespace DoctorAppointment.DTO
{
    public class ScheduleDTO
    {
        public string SelectDay { get; set; }
        [Required]
        public DateTime[] StartTime { get; set; }
        [Required]
        public DateTime[] EndTime { get; set;}
        public string CombinedTimes { get; set; } 

    }
}
