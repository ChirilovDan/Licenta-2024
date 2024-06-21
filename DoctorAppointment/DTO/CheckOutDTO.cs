#nullable disable
namespace DoctorAppointment.DTO
{
    public class CheckOutDTO
    {
        public string SelectTime { get; set; }  
        public decimal Fee { get; set; }  
        public string Docid { get; set; }  
        public string Day { get; set; }  
        public string CardHolderName { get; set; }  
        public string CardNumber { get; set; }  
        public DateTime CardExpiryDate { get; set; }  
        public string CardCVV { get; set; }  
        public string ServiceList { get; set; }
    }
}
