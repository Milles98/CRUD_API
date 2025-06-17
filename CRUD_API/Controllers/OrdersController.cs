using CRUD_API.Data;
using Microsoft.AspNetCore.Http;
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
    }
}
