using ProductService.Model.DTO;

namespace ProductService.Repository
{
    public interface ICategoryService
    {
        List<CategoryDto> GetCategories();
        void AddNewCatrgory(CategoryDto category);
    }
}
