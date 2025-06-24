using CRUD_API.Enums;

namespace CRUD_API.Models
{
    public class Payment
    {
        public int Id { get; set; }
        public DateTime AttemptedAt { get; set; }
        public decimal Amount { get; set; }
        public PaymentMethod Method { get; set; }
        public PaymentStatus Status { get; set; }

        //FK
        public int OrderId { get; set; }
        public Order? Order { get; set; }
    }
}
