using App.Metrics;
using Microsoft.AspNetCore.Mvc;
using ProductService.Repository;

namespace ProductService.Controllers
{
    [ApiController]
    [Route("api/[Controller]")]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;
        // private readonly IMetrics _metrics;
        private readonly ILogger<ProductController> _logger;
        public ProductController(IProductService productService, ILogger<ProductController> logger)
        {
            _productService = productService;
            // _metrics = metrics;
            _logger = logger;
        }

        [HttpGet]
        public IActionResult Get()
        {
            // _metrics.Measure.Counter.Increment(new App.Metrics.Counter.CounterOptions
            // {
            //     Name = "get_list_producet"
            // });

            var data = _productService.GetProductList();
            return Ok(data);
        }

        [HttpGet("{id}")]
        public IActionResult Get(Guid id)
        {
            // _metrics.Measure.Counter.Increment(new App.Metrics.Counter.CounterOptions
            // {
            //     Name = "get_detail_producet_id"
            // });

            var data = _productService.GetProduct(id);
            _logger.LogInformation("GET PRODUCT");
            return Ok(data);
        }
    }
}