#nullable disable
using DoctorAppointment.Models;

namespace DoctorAppointment.ViewModel
{
    public class BookingViewModel
    {
        public string  DoctorName {  get; set; }    
        public string State { get; set; }   
        public string Contry {  get; set; } 
        public string Image {  get; set; }  
        public string userid { get; set; }
        public List<ScheduleTiming> scheduleTimings { get; set; }   
        public List<OfferServices> OfferServices { get; set; }   
    }
}
