using CRUD_API.Data;
using CRUD_API.DTO;
using CRUD_API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CRUD_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController(AppDbContext context) : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> GetOrder(int id)
        {
            var order = await context.Orders.FirstOrDefaultAsync(o => o.Id == id);

            if (order != null)
                return Ok(order);

            return NotFound();
        }

        [HttpGet("debug-data")]
        public async Task<IActionResult> DebugData()
        {
            var customers = await context.Customers.ToListAsync();
            var products = await context.Products.ToListAsync();

            return Ok(new { customers, products });
        }


        [HttpPost]
        public async Task<IActionResult> CreateOrder(CreateOrderDTO orderDto)
        {
            var customer = await context.Customers.FirstOrDefaultAsync(o => o.Id == orderDto.CustomerId);
            var listOfProducts = await context.Products.Where(p => orderDto.ProductIds.Contains(p.Id)).ToListAsync();
            var orderTotal = listOfProducts.Sum(p => p.Price);
            
            if(customer != null && listOfProducts != null)
            {
                var order = new Order
                {
                    Customer = customer,
                    CustomerId = orderDto.CustomerId,
                    Products = listOfProducts,
                    CreatedAt = DateTime.UtcNow,
                    TotalAmount = orderTotal,
                    Payments = null
                };

                var response = new OrderResponseDto
                {
                    Id = order.Id,
                    CreatedAt = order.CreatedAt,
                    TotalAmount = order.TotalAmount,
                    CustomerName = order.Customer.FullName,
                    ProductNames = listOfProducts.Select(p => p.Name).ToList()
                };

                await context.Orders.AddAsync(order);
                await context.SaveChangesAsync();

                return CreatedAtAction(nameof(GetOrder), new { id = order.Id}, order);
            }
            return BadRequest();
        }
    }
}
