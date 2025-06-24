using CRUD_API.Enums;
using System.ComponentModel.DataAnnotations;

namespace CRUD_API.DTO
{
    public class CreatePaymentDto
    {
        [Required]
        public PaymentMethod Method { get; set; }
        [Required]
        public int OrderId { get; set; }
    }
}
