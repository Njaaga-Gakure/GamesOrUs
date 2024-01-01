using AutoMapper;
using CouponService.Models;
using CouponService.Models.DTOs;
using CouponService.Service.IService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Stripe;

namespace CouponService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CouponController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly ICoupon _couponService;
        private readonly ResponseDTO _response;

        public CouponController(IMapper mapper, ICoupon couponService)
        {
            _mapper = mapper;
            _couponService = couponService;
            _response = new ResponseDTO();
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ResponseDTO>> AddCoupon(CouponDTO coupon)
        {
            try
            {
                // clashes with a stripe Class
                var newCoupon = _mapper.Map<Models.Coupon>(coupon);
                var response = await _couponService.AddCoupon(newCoupon);

                // adding coupon to stripe
                var options = new CouponCreateOptions()
                {
                    AmountOff = (long)newCoupon.CouponDiscount * 100,
                    Currency = "kes",
                    Id = newCoupon.CouponCode,
                    Name = newCoupon.CouponCode
                };

                var service = new Stripe.CouponService();
                service.Create(options); 

                _response.Result = response;
                return Ok(_response);

            }
            catch (Exception ex)
            {
                _response.ErrorMessage = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                return StatusCode(500, _response);
            }
        }

        [HttpGet]
        [Authorize]
        public async Task<ActionResult<ResponseDTO>> GetAllCoupons()
        {
            try
            {
                var coupons = await _couponService.GetAllCoupons();
                _response.Result = coupons;
                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.ErrorMessage = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                return StatusCode(500, _response);
            }
        }

        [HttpGet("{couponId}")]
        [Authorize]
        public async Task<ActionResult<ResponseDTO>> GetCouponById(Guid couponId)
        {
            try
            {
                var coupon = await _couponService.GetCouponById(couponId);
                if (coupon == null)
                {
                    _response.ErrorMessage = "Coupon Not Found :(";
                    return NotFound(_response);
                }
                _response.Result = coupon;
                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.ErrorMessage = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                return StatusCode(500, _response);
            }
        }
        [HttpGet("code/{couponCode}")]
        [Authorize]
        public async Task<ActionResult<ResponseDTO>> GetCouponByCouponCode(string couponCode)
        {
            try
            {
                var coupon = await _couponService.GetCouponByCouponCode(couponCode);
                if (coupon == null)
                {
                    _response.ErrorMessage = "Coupon Not Found :(";
                    return NotFound(_response);
                }
                _response.Result = coupon;
                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.ErrorMessage = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                return StatusCode(500, _response);
            }
        }

        [HttpPatch("{couponId}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ResponseDTO>> UpdateCoupon(Guid couponId, CouponDTO updateCoupon)
        {
            try
            {
                var coupon = await _couponService.GetCouponById(couponId);
                if (coupon == null)
                {
                    _response.ErrorMessage = "Coupon Not Found :(";
                    return NotFound(_response);
                }
                var previousCouponCode = coupon.CouponCode;

                await _couponService.UpdateCoupon(couponId, updateCoupon);
                // updating a coupon from stripe

                // delete coupon first
                var service = new Stripe.CouponService();
                service.Delete(previousCouponCode);

                // add a new coupon with updated values
                var options = new CouponCreateOptions()
                {
                    AmountOff = (long) updateCoupon.CouponDiscount * 100,
                    Currency = "kes",
                    Id = updateCoupon.CouponCode,
                    Name = updateCoupon.CouponCode
                };
                service.Create(options);

                _response.Result = "Coupon Updated Successfully :)";
                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.ErrorMessage = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                return StatusCode(500, _response);
            }
        }

        [HttpDelete("{couponId}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ResponseDTO>> DeleteCoupon(Guid couponId)
        {
            try
            {
                var coupon = await _couponService.GetCouponById(couponId);
                if (coupon == null)
                {
                    _response.ErrorMessage = "Coupon Not Found :(";
                    return NotFound(_response);
                }
                await _couponService.DeleteCoupon(couponId);

                // deleting coupon from stripe

                var service = new Stripe.CouponService();
                service.Delete(coupon.CouponCode);


                _response.Result = "Coupon Deleted Successfully :)";
                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.ErrorMessage = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                return StatusCode(500, _response);
            }
        }
    }
}
