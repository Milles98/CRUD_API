using CRUD_API.Models;

namespace CRUD_API.DTO
{
    public class OrderUpdateDto
    {
        public int CustomerId { get; set; }

        public List<int> ProductIds { get; set; }
    }
}
