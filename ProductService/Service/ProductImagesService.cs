using ProductService.Data;
using ProductService.Models;
using ProductService.Service.IService;

namespace ProductService.Service
{
    public class ProductImagesService : IProductImage
    {
        private readonly ProductContext _context;

        public ProductImagesService(ProductContext context)
        {
            _context = context;
        }
        public async Task<string> AddProductImage(ProductImage image)
        {
            await _context.ProductImages.AddAsync(image);   
            await _context.SaveChangesAsync();
            return "Product Image Added Succefully :)";
        }
    }
}
