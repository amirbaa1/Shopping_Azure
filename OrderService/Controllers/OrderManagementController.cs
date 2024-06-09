using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace OrderService.Controllers
{
    [ApiController]
    [Route("api/OrderManagement")]
    [Authorize(Policy = "ManagementOrder")]
    public class OrderManagementController : ControllerBase
    {
        [HttpPut("{id}")]
        public IActionResult Edit(Guid id)
        {
            return Ok(true);
        }
    }
}
