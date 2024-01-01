using ProductService.Models;
using ProductService.Models.DTOs;

namespace ProductService.Service.IService
{
    public interface IProduct
    {
        Task<string> AddProduct(Product product);
        Task<List<ProductResponseDTO>> GetAllProducts();
        Task<ProductResponseDTO> GetProductById(Guid productId);
        Task<bool> UpdateProduct(Guid productId, ProductDTO updateProduct);
        Task<bool> DeleteProduct(Guid productId);

    }
}
