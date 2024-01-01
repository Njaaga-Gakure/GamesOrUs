using CartService.Models;
using Microsoft.EntityFrameworkCore;

namespace CartService.Data
{
    public class CartContext : DbContext
    {
        public CartContext(DbContextOptions<CartContext> options) : base (options) {}

        public DbSet<Cart> Carts { get; set; }  
        public DbSet<CartItem> CartItems { get; set; }
        public DbSet<CartItemImages> CartItemImages { get; set; }   
     }
}
