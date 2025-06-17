namespace CRUD_API.DTO
{
    public class CreateOrderDTO
    {
        public int CustomerId { get; set; }
        public required List<int> ProductIds { get; set; }
    }
}
