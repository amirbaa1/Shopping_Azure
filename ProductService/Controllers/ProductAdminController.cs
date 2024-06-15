using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProductService.MessageBus;
using ProductService.MessageBus.Message;
using ProductService.Model.DTO;
using ProductService.Repository;

namespace ProductService.Controllers
{
    [ApiController]
    [Route("api/[Controller]")]
    [Authorize(Policy = "ProductAdmin")]
    public class ProductAdminController : ControllerBase
    {
        private readonly IProductService _productService;
        private readonly IMessageBus _messageBus;
        private readonly IConfiguration _configuration;
        private readonly ILogger<ProductAdminController> _logger;
        private readonly ICategoryService _categoryService;

        public ProductAdminController(IProductService productService, IMessageBus messageBus,
            ILogger<ProductAdminController> logger, ICategoryService categoryService)
        {
            _productService = productService;
            _messageBus = messageBus;
            _logger = logger;
            _categoryService = categoryService;
        }

        [HttpPost("category")]
        public IActionResult Post([FromBody] CategoryDto categoryDto)
        {
            _categoryService.AddNewCatrgory(categoryDto);
            return Ok();
        }

        [HttpPost("product")]
        public async Task<IActionResult> PostProduct([FromBody] AddNewProductDto addNewProductDto)
        {
            var productCheck = await _productService.AddProduct(addNewProductDto);
            if (productCheck == null)
            {
                return BadRequest();
            }

            return Ok("Add Product");
        }

        [HttpPut]
        public async Task<IActionResult> UpdateProduct(UpdateProductDto updateProductDto,
            [FromServices] IMessageBus messageBus)
        {
            var updateProduct = _productService.UpdateProductName(updateProductDto);
            if (updateProduct != null)
            {
                var updateMessageRabbit = new UpdateProductRabbitMq
                {
                    CreateTime = DateTime.UtcNow,
                    MessageId = Guid.NewGuid(),
                    Name = updateProduct.Name,
                    Price = updateProduct.Price,

                    // ProductId = updateProductDto.ProductId,
                    ProductId = updateProduct.Id,
                };
                _messageBus.SandMessage(updateMessageRabbit, "Update-ProductName");
                return Ok(updateProduct);
            }

            return NotFound(updateProductDto);
        }

        [HttpDelete("{productId}")]
        public IActionResult DeleteProduct(Guid productId)
        {
            var deletePro = _productService.DeleteProduct(productId);
            if (deletePro == false)
            {
                _logger.LogError("NOT Found");
                return NotFound("پیدا نشد.");
            }

            return Ok("حذف شد.");
        }
    }
}