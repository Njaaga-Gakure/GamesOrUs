using CouponService.Models;
using CouponService.Models.DTOs;

namespace CouponService.Service.IService
{
    public interface ICoupon
    {
        Task<string> AddCoupon(Coupon coupon);
        Task<List<Coupon>> GetAllCoupons();

        Task<Coupon> GetCouponById(Guid Id);

        Task<Coupon> GetCouponByCouponCode(string code);

        Task UpdateCoupon(Guid Id, CouponDTO updateCoupon);

        Task DeleteCoupon(Guid Id);
    }
}
