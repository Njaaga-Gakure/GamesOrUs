using ProductService.Models;

namespace ProductService.Service.IService
{
    public interface IProductImage
    {
        Task<string> AddProductImage(ProductImage image);
    }
}
