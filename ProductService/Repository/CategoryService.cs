using ProductService.Data;
using ProductService.Model;
using ProductService.Model.DTO;

namespace ProductService.Repository
{
    public class CategoryService : ICategoryService
    {
        private readonly ProductdbContext _dbContext;

        public CategoryService(ProductdbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public void AddNewCatrgory(CategoryDto category)
        {
            Category newCategory = new Category
            {
                Description = category.Description,
                Name = category.Name,
            };
            _dbContext.Categories.Add(newCategory);
            _dbContext.SaveChanges();
        }

        public List<CategoryDto> GetCategories()
        {
            var data = _dbContext.Categories
                        .OrderBy(p => p.Name)
                        .Select(p => new CategoryDto
                        {
                            Description = p.Description,
                            Name = p.Name,
                            Id = p.Id
                        }).ToList();
            return data;
        }
    }
}
