using CouponService.Models;
using Microsoft.EntityFrameworkCore;

namespace CouponService.Data
{
    public class CouponContext : DbContext
    {
        public CouponContext(DbContextOptions<CouponContext> options) : base (options) {}
        public DbSet<Coupon> Coupons { get; set; }  
    }
}
