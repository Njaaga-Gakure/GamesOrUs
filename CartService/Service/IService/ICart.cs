using CartService.Models;
using CartService.Models.DTOs;

namespace CartService.Service.IService
{
    public interface ICart
    {
        Task CreateCart(Cart cart);
        Task<CartResponseDTO> GetCartByUserId(Guid UserId);
        Task AddToCart(CartItem Item);
        Task UpdateCartTotals(Guid cartId, decimal amount);
        Task UpdateCartItemQuantity(Guid id, int quantity);

        Task UpdateCartCouponDetails(Guid id, decimal discount, string? couponCode = null);
        Task<bool> RemoveProductFromCart(Guid productId);
        Task<bool> DeleteCart(Guid cartId);
    }
}
