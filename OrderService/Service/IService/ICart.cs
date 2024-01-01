using OrderService.Models.DTOs;

namespace OrderService.Service.IService
{
    public interface ICart
    {
        Task<CartDTO> GetCartByUserId(Guid UserId, string token);
        Task<bool> DeleteCart(Guid UserId, string token);
    }
}
