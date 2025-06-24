using CRUD_API.Data;
using CRUD_API.DTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CRUD_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentsController(AppDbContext context) : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> GetAllPayments()
        {
            var payments = await context.Payments
                .ToListAsync();

            if(payments.Count == 0)
                return Ok(new List<PaymentResponseDto>());

            var paymentDto = payments.Select(payment => new PaymentResponseDto
            {
                Id = payment.Id,
                AttemptedAt = payment.AttemptedAt,
                Amount = payment.Amount,
                Method = payment.Method,
                OrderId = payment.OrderId,
                Status = payment.Status
            });

            return Ok(paymentDto);
        }
    }
}
