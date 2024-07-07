namespace Backend_DV_YTe.Model
{
    public class Invoice
    {
        public string CustomerName { get; set; }
        public string? PhoneNumberCustomer { get; set; }
        public string? LocationCustomer { get; set; }
        public string DoctorName { get; set; }
        public string ServiceName { get; set; }
        public int MaDV { get; set; }
        public string Location { get; set; }
        public DateTime AppointmentTime { get; set; }
        public double TotalAmount { get; set; }
        public DateTime PaymentDate { get; set; }
        public string PaymentMethod { get; set; }
        public string PaymentStatus { get; set; }
    }
}
