using CartService.Models.DTOs;

namespace CartService.Service.IService
{
    public interface ICoupon
    {
        Task <CouponDTO> GetCouponByCouponCode(string couponCode, string token);
    }
}
