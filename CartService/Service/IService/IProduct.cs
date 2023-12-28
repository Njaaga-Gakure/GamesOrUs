using CartService.Models.DTOs;

namespace CartService.Service.IService
{
    public interface IProduct
    {
        Task<ProductDTO> GetProductById(Guid productId);
    }
}
