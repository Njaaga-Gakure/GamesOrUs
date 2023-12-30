using AutoMapper;
using CouponService.Models;
using CouponService.Models.DTOs;

namespace CouponService.Profiles
{
    public class CouponProfiles : Profile
    {
        public CouponProfiles()
        {
            CreateMap<CouponDTO, Coupon>().ReverseMap();    
        }
    }
}
