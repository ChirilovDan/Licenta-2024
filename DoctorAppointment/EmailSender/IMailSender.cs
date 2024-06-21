namespace DoctorAppointment.EmailSender
{
    public interface IMailSender
    {
        public void MessageSend(Message message);
    }
}
