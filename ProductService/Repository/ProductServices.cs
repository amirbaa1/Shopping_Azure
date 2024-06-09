using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using ProductService.Data;
using ProductService.Model;
using ProductService.Model.DTO;

namespace ProductService.Repository
{
    public class ProductServices : IProductService
    {
        private readonly ProductdbContext _dbContext;
        private readonly ILogger<ProductServices> _logger;

        public ProductServices(ProductdbContext dbContext, ILogger<ProductServices> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public async Task<string> AddProduct(AddNewProductDto addNewProductDto)
        {
            //var category = await _dbContext.Categories.Where(x => x.Id == addNewProductDto.CategoryId)
            //    .FirstOrDefaultAsync();

            var category = await _dbContext.Categories.FindAsync(addNewProductDto.CategoryId);

            _logger.LogInformation($"---> product category : {category}");
            if (category == null)
            {
                return "Not found category";
            }

            var product = new Product()
            {
                Category = category,
                Description = addNewProductDto.Description,
                Image = addNewProductDto.Image,
                Name = addNewProductDto.Name,
                Price = addNewProductDto.Price
            };
            if (product == null)
            {
                return "اضافه نشد در دیتابیس";
            }
            else
            {
                _dbContext.Products.Add(product);
                await _dbContext.SaveChangesAsync();
                return "اضافه شد در دیتابیس";
            }
        }

        public async Task<ProductDto> GetProduct(Guid Id)
        {
            // var product = await _dbContext.Products.Where(x => x.Id == Id)
            // .FirstOrDefaultAsync();
            var product = _dbContext.Products.Include(p => p.Category)
                .SingleOrDefault(p => p.Id == Id);

            _logger.LogInformation($"product sear===>{product}");
            if (product == null)
            {
                throw new Exception("Product Note Found...!");
            }

            var productGetID = new ProductDto()
            {
                Description = product.Description,
                Name = product.Name,
                Price = product.Price,
                Id = product.Id,
                Image = product.Image,
                productCategory = new ProductCategoryDto
                {
                    Category = product.Category.Name,
                    CategoryId = product.Category.Id
                },
            };
            _logger.LogInformation($"----> {productGetID}");
            return productGetID;
            //return product;
        }

        public Product UpdateProductName(UpdateProductDto updateProduct)
        {
            var getProduct = _dbContext.Products.Find(updateProduct.ProductId);
            if (getProduct == null)
            {
                return null;
            }

            if (updateProduct.Name == "string" || string.IsNullOrWhiteSpace(updateProduct.Name))
            {
                getProduct.Name = getProduct.Name;
            }
            else
            {
                getProduct.Name = updateProduct.Name;
            }

            if (updateProduct.Price == 0)
            {
                getProduct.Price = getProduct.Price;
            }
            else
            {
                getProduct.Price = updateProduct.Price;
            }

            _dbContext.SaveChanges();
            return getProduct;
        }

        public bool DeleteProduct(Guid productId)
        {
            var getpro = _dbContext.Products.SingleOrDefault(x => x.Id == productId);
            if (getpro == null)
            {
                return false;
            }

            _dbContext.Products.Remove(getpro);
            _dbContext.SaveChanges();
            return true;
        }

        public Task<List<ProductDto>> GetProductList()
        {
            var data = _dbContext.Products
                .OrderByDescending(p => p.Id)
                .Select(p => new ProductDto
                {
                    Description = p.Description,
                    Id = p.Id,
                    Image = p.Image,
                    Name = p.Name,
                    Price = p.Price,
                    productCategory = new ProductCategoryDto
                    {
                        Category = p.Category.Name,
                        CategoryId = p.Category.Id
                    }
                }).ToList();

            return Task.FromResult(data);
        }
    }
}