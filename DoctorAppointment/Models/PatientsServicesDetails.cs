using System.ComponentModel.DataAnnotations;

namespace DoctorAppointment.Models
{
    public class PatientsServicesDetails
    {
        [Key]
        public int Id { get; set; }
        public int servicesID { get; set; } 
        public int Orderid { get; set; }    
    }
}
