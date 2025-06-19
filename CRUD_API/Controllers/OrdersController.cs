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
        [HttpGet("{id}")]
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

        [HttpGet]
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

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateOrder(int id, OrderUpdateDto orderUpdateDto)
        {
            //Validera att kunden finns
            var customer = await context.Customers.FindAsync(orderUpdateDto.CustomerId);
            if (customer == null)
                return BadRequest($"Could not find customer with id {id}");

            var products = await context.Products.Where(p => orderUpdateDto.ProductIds.Contains(p.Id)).ToListAsync();
            if (products.Count != orderUpdateDto.ProductIds.Count)
                return BadRequest($"One or more product ids invalid");

            var order = await context.Orders.Include(p => p.Products).FirstOrDefaultAsync(o => o.Id == id);
            if (order == null)
                return NotFound("Could not find order");

            order.CustomerId = customer.Id;
            order.Products = products;
            order.TotalAmount = products.Sum(p => p.Price);

            await context.SaveChangesAsync();

            var updatedDto = new OrderResponseDto
            {
                Id = order.Id,
                CreatedAt = order.CreatedAt,
                CustomerName = order.Customer?.FullName,
                ProductNames = products.Select(p => p.Name!).ToList(),
                TotalAmount = order.TotalAmount
            };

            return Ok(updatedDto);
        }

        [HttpPost]
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

        [HttpDelete("{id}")]
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
