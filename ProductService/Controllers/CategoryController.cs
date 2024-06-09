using App.Metrics;
using Microsoft.AspNetCore.Mvc;
using ProductService.Model.DTO;
using ProductService.Repository;

namespace ProductService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryService _categoryService;
        private readonly ILogger<CategoryController> _logger;
        private readonly IMetrics _metrics;

        public CategoryController(ICategoryService categoryService, ILogger<CategoryController> logger, IMetrics metrics)
        {
            _categoryService = categoryService;
            _logger = logger;
            _metrics = metrics;
        }

        [HttpGet]
        public IActionResult Get()
        {

            _metrics.Measure.Counter.Increment(new App.Metrics.Counter.CounterOptions
            {
                Name = "get_list_producet_category"
            });


            var data = _categoryService.GetCategories();
            _logger.LogInformation("-------");
            _logger.LogWarning("Log warning");
            _logger.LogError("ERoror");
            return Ok(data);
        }

        [HttpPost]
        public IActionResult Post([FromBody] CategoryDto categoryDto)
        {
            _categoryService.AddNewCatrgory(categoryDto);
            return Ok();
        }
    }
}