#nullable disable
namespace DoctorAppointment.ViewModel
{
    public class SummeryviewModel
    {
        public string Image {  get; set; }  
        public DateTime Date { get; set; }  
        public string Day { get; set; }
        public string Email { get; set; }   
        public decimal Fee { get; set; } 
        public string Name { get; set; }
        public string State { get; set; }
        public string Country {  get; set; }
        public string SelectTime {  get; set; } 
        public string Docid {  get; set; }  

        public decimal TotalFee { get; set; }

        public string ServiceList {  get; set; }    
    }


}
