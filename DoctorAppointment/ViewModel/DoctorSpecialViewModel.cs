#nullable disable
using DoctorAppointment.DTO;
using DoctorAppointment.Models;

namespace DoctorAppointment.ViewModel
{
    public class DoctorSpecialViewModel
    {
        public List<UserDto> Users { get; set; }
        public List<Specialities> Specialities { get; set; }    
    }
}
