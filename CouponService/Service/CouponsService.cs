using CouponService.Data;
using CouponService.Models;
using CouponService.Models.DTOs;
using CouponService.Service.IService;
using Microsoft.EntityFrameworkCore;

namespace CouponService.Service
{
    public class CouponsService : ICoupon
    {
        private readonly CouponContext _context;

        public CouponsService(CouponContext context)
        {
            _context = context;
        }
        public async Task<string> AddCoupon(Coupon coupon)
        {
            await _context.Coupons.AddAsync(coupon);
            await _context.SaveChangesAsync();
            return "Coupon Added Successfully :)";
        }

        public async Task<List<Coupon>> GetAllCoupons()
        {
            return await _context.Coupons.ToListAsync();
        }

        public async Task<Coupon> GetCouponByCouponCode(string code)
        {
            return await _context.Coupons.Where(coupon => coupon.CouponCode == code).FirstOrDefaultAsync();
        }

        public async Task<Coupon> GetCouponById(Guid Id)
        {
            return await _context.Coupons.Where(coupon => coupon.Id == Id).FirstOrDefaultAsync();
        }

        public async Task<bool> UpdateCoupon(Guid Id, CouponDTO updateCoupon)
        {
            var coupon = await GetCouponById(Id);
            if (coupon != null)
            {
                coupon.CouponCode = updateCoupon.CouponCode;
                coupon.CouponDiscount = updateCoupon.CouponDiscount;
                coupon.CouponMinimumAmount = updateCoupon.CouponMinimumAmount;
                await _context.SaveChangesAsync();
                return true;
            }
            return false;
        }

        public async Task<bool> DeleteCoupon(Guid Id)
        {
            var coupon = await GetCouponById(Id);

            if (coupon != null)
            {
                _context.Coupons.Remove(coupon);
                await _context.SaveChangesAsync();
                return true;
            }
            return false;
        }

    }
}
