#nullable disable
using System.ComponentModel.DataAnnotations;

namespace DoctorAppointment.Models
{
    public class PatientsOrder
    {
        [Key]
        public int Id { get; set; }
        public string DocID { get; set; }  
        public string SelectTime { get; set; }    
        public DateTime OrderDate { get; set; } 
        public decimal TotalPay { get; set; }   
        public string UserID { get; set; } 
        public string Status {  get; set; } 

        public string SelectDay { get; set; }   

    }
}
