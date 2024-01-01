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
                                .ThenInclude(cartItem => cartItem.ProductImages)
                                .Select(cart => new CartResponseDTO()
                                {
                                    CartId = cart.Id,
                                    UserId = cart.UserId,
                                    CouponCode = cart.CouponCode,
                                    Discount = cart.Discount,
                                    CartItems = _mapper.Map<List<CartItemDTO>>(cart.CartItems.Select(cartItem => new CartItemDTO()
                                    {
                                        Id = cartItem.Id,
                                        ProductId = cartItem.ProductId,
                                        ProductName = cartItem.ProductName,
                                        ProductDescription = cartItem.ProductDescription,
                                        ProductGenre = cartItem.ProductGenre,
                                        ProductUnitPrice = cartItem.ProductUnitPrice,
                                        ProductQuantity = cartItem.ProductQuantity,
                                        ProductImage = cartItem.ProductImages[0].Image
                                    })),
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

        public async Task RemoveProductFromCart(Guid productId)
        {
            var cartItem = await _context.CartItems.Where(cartItem => cartItem.ProductId == productId).FirstOrDefaultAsync();

            _context.CartItems.Remove(cartItem);
            await _context.SaveChangesAsync();

        }

        public async Task UpdateCartCouponDetails(Guid id, decimal discount, string couponCode)
        {
            var cart = await _context.Carts.Where(cart => cart.Id == id).FirstOrDefaultAsync();
            cart.CouponCode = couponCode;
            cart.Discount = discount;
            await _context.SaveChangesAsync();
        }
    }
}
