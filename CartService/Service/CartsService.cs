using AutoMapper;
using CartService.Data;
using CartService.Models;
using CartService.Models.DTOs;
using CartService.Service.IService;
using Microsoft.EntityFrameworkCore;

namespace CartService.Service
{
    public class CartsService : ICart
    {
        private readonly CartContext _context;
        private readonly IMapper _mapper;

        public CartsService(CartContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;

        }
        public async Task CreateCart(Cart cart)
        {
            _context.Carts.Add(cart);
            await _context.SaveChangesAsync();
        }
        public async Task AddToCart(CartItem cartItem)
        {
            await _context.CartItems.AddAsync(cartItem);
        }
        public async Task<CartResponseDTO> GetCartByUserId(Guid UserId)
        {
            var cart = await _context.Carts
                                .Where(cart => cart.UserId == UserId)
                                .Include(cart => cart.CartItems)
                                .Select(cart => new CartResponseDTO() {
                                    CartId = cart.Id,
                                    UserId = cart.UserId,
                                    CartItems = _mapper.Map<List<CartItemDTO>>(cart.CartItems.ToList()),
                                    TotalAmount = cart.TotalAmount
                                })
                                .FirstOrDefaultAsync();
            return cart;
        }

        public async Task UpdateCartTotals(Guid cartId, decimal amount)
        {
            var cart = await _context.Carts.Where(cart => cart.Id == cartId).FirstOrDefaultAsync();
            cart.TotalAmount += amount;
            await _context.SaveChangesAsync();  
        }
        public async Task<bool> DeleteCart(Guid cartId)
        {
            var cart = await _context.Carts.Where(cart => cart.Id == cartId).FirstOrDefaultAsync();
            if (cart != null)
            {
                _context.Carts.Remove(cart);
                await _context.SaveChangesAsync();
                return true;
            }
            return false;  
        }

        public async Task UpdateCartItemQuantity(Guid id, int quantity)
        {
            var cartItem = await _context.CartItems.Where(cartItem => cartItem.Id == id).FirstOrDefaultAsync();
            cartItem.ProductQuantity = quantity;
            await _context.SaveChangesAsync();  
        }
    }
}
