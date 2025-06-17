namespace CRUD_API.Models
{
    public class Customer
    {
        public int Id { get; set; }
        public string? Email { get; set; }
        public string? FullName { get; set; }

        public ICollection<Order>? Orders { get; set; }
    }
}
