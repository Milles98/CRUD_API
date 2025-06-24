namespace CRUD_API.DTO
{
    public class PaymentResponseDto
    {
        public int Id { get; set; }
        public DateTime AttemptedAt { get; set; }
        public decimal Amount { get; set; }
        public string? Method { get; set; }
        public string? Status { get; set; }
        public int OrderId { get; set; }
    }
}
