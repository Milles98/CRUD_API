namespace CRUD_API.DTO
{
    public class OrderResponseDto
    {
        public int Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public decimal TotalAmount { get; set; }
        public string? CustomerName { get; set; }
        public List<string>? ProductNames { get; set; }
    }
}
