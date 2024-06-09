using WebApp.Admin.Model.Dto;

namespace WebApp.Admin.Services;

public interface IProductManagement
{
    Task<List<ProductDto>> GetListProduct();
    Task<ResultDto> UpdateProduct(UpdateProductDto updateProductDto);

    Task<ResultDto> DeleteProduct(Guid produtId);
}