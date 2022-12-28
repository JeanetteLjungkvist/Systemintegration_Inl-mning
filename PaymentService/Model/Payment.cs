namespace PaymentService.Model{
    
    public class Payment {

        public Guid Id { get; set; }

        public string? CustomerId { get; set; }

        public int Total { get; set; }

        public DateTime Date { get; set; }
    }
}