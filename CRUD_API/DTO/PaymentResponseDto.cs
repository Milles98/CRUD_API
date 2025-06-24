using CRUD_API.Enums;

namespace CRUD_API.DTO
{
    public class PaymentResponseDto
    {
        public int Id { get; set; }
        public DateTime AttemptedAt { get; set; }
        public decimal Amount { get; set; }
        public PaymentMethod Method { get; set; }
        public PaymentStatus Status { get; set; }
        public int OrderId { get; set; }
    }
}
