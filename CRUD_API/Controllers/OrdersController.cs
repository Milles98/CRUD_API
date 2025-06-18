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
        [HttpGet("Get Order")]
        public async Task<IActionResult> GetOrder(int id)
        {
            var order = await context.Orders
                .Include(c => c.Customer)
                .Include(p => p.Products)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (order == null)
                return NotFound();

            var orderDto = new OrderResponseDto
            {
                Id = order.Id,
                CreatedAt = order.CreatedAt,
                CustomerName = order.Customer?.FullName,
                ProductNames = order.Products!.Select(p => p.Name!).ToList(),
                TotalAmount = order.TotalAmount
            };

            return Ok(orderDto);
        }

        [HttpGet("Get All Orders")]
        public async Task<IActionResult> GetAllOrders()
        {
            var orders = await context.Orders
                .Include(c => c.Customer)
                .Include(p => p.Products)
                .ToListAsync();

            var orderDtos = orders
                .Select(order => new OrderResponseDto
                {
                    Id = order.Id,
                    CreatedAt = order.CreatedAt,
                    CustomerName = order.Customer?.FullName,
                    ProductNames = order.Products!.Select(p => p.Name!).ToList(),
                    TotalAmount = order.TotalAmount,
                }).ToList();

            if (orderDtos.Count > 0)
            return Ok(orderDtos);

            return Ok("Found no orders");
        }

        //[HttpGet("debug-data")]
        //public async Task<IActionResult> DebugData()
        //{
        //    var customers = await context.Customers.ToListAsync();
        //    var products = await context.Products.ToListAsync();

        //    return Ok(new { customers, products });
        //}

        //[HttpPut("Update Order")]
        //public async Task<IActionResult> UpdateOrder()
        //{

        //}

        [HttpPost("Create Order")]
        public async Task<IActionResult> CreateOrder(CreateOrderDTO orderDto)
        {
            var customer = await context.Customers.FirstOrDefaultAsync(o => o.Id == orderDto.CustomerId);
            var listOfProducts = await context.Products.Where(p => orderDto.ProductIds.Contains(p.Id)).ToListAsync();
            var orderTotal = listOfProducts.Sum(p => p.Price);

            if (customer != null && listOfProducts.Count == orderDto.ProductIds.Count)
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

                await context.Orders.AddAsync(order);
                await context.SaveChangesAsync();

                var response = new OrderResponseDto
                {
                    Id = order.Id,
                    CreatedAt = order.CreatedAt,
                    TotalAmount = order.TotalAmount,
                    CustomerName = order.Customer.FullName,
                    ProductNames = listOfProducts.Select(p => p.Name!).ToList()
                };

                return CreatedAtAction(nameof(GetOrder), new { id = response.Id }, response);
            }
            return BadRequest("Invalid customer or product IDs not found");
        }

        [HttpDelete("Delete Order")]
        public async Task<IActionResult> DeleteOrder(int id)
        {
            var order = await context.Orders.FirstOrDefaultAsync(o => o.Id == id);

            if (order == null)
                return BadRequest();
            
            context.Orders.Remove(order);
            await context.SaveChangesAsync();

            return NoContent();
        }
    }
}
