using CRUD_API.Data;
using CRUD_API.DTO;
using CRUD_API.Enums;
using CRUD_API.Models;
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

            if (payments.Count == 0)
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

        [HttpGet("{id}")]
        public async Task<IActionResult> GetPayment(int id)
        {
            var payment = await context.Payments.Where(p => p.Id == id).FirstOrDefaultAsync();

            if (payment == null)
                return NotFound();

            var paymentDto = new PaymentResponseDto
            {
                Id = payment.Id,
                AttemptedAt = payment.AttemptedAt,
                Amount= payment.Amount,
                Method = payment.Method,
                OrderId = payment.OrderId,
                Status = payment.Status
            };

            return Ok(paymentDto);
        }

        [HttpPost]
        public async Task<IActionResult> CreatePayment([FromBody]CreatePaymentDto paymentDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var order = await context.Orders.FirstOrDefaultAsync(o => o.Id == paymentDto.OrderId);

            if (order == null)
                return BadRequest("Order invalid");

            var amount = order.TotalAmount;

            var payment = new Payment
            {
                Amount = amount,
                Status = PaymentStatus.Pending,
                AttemptedAt = DateTime.UtcNow,
                Method = paymentDto.Method,
                OrderId = paymentDto.OrderId,
            };

            await context.Payments.AddAsync(payment);
            await context.SaveChangesAsync();

            var response = new PaymentResponseDto
            {
                Id = payment.Id,
                AttemptedAt = payment.AttemptedAt,
                Amount = payment.Amount,
                Method = payment.Method,
                OrderId = payment.OrderId,
                Status = payment.Status
            };

            return CreatedAtAction(nameof(GetPayment), new { id = response.Id}, response);
        }

        [HttpDelete]
        public async Task<IActionResult> DeletePayment(int id)
        {
            var payment = await context.Payments.Where(c => c.Id == id).FirstOrDefaultAsync();

            if (payment == null)
                return BadRequest();

            context.Payments.Remove(payment);
            await context.SaveChangesAsync();
            return Ok();
        }
    }
}
