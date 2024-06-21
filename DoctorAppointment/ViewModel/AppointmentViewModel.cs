#nullable disable
namespace DoctorAppointment.ViewModel
{
    public class AppointmentViewModel
    {
        public string Name { get; set; }    
        public string Image {  get; set; }  
        public DateTime AppointmentDate {  get; set; }    
        public string BookingDate {  get; set; }
        public decimal Amount {  get; set; } 

        public string Status {  get; set; } 
        public string Special {  get; set; }    
        public string Time {  get; set; }  
        
        public string PatientId { get; set; }
        public int id { get; set; }
    }
}
