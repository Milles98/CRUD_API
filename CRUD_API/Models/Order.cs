namespace CRUD_API.Models
{
    public class Order
    {
        public int Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public decimal TotalAmount { get; set; }

        //Foreign Keys
        public int CustomerId { get; set; }
        public Customer? Customer { get; set; }

        public ICollection<Product>? Products { get; set; }

        //Navigation
        public ICollection<Payment>? Payments { get; set; }
    }
}
