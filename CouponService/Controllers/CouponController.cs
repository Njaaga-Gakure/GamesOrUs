using AutoMapper;
using CouponService.Models;
using CouponService.Models.DTOs;
using CouponService.Service.IService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

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
                var newCoupon = _mapper.Map<Coupon>(coupon);
                var response = await _couponService.AddCoupon(newCoupon);
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
                var couponExists = await _couponService.UpdateCoupon(couponId, updateCoupon);
                if (!couponExists)
                {
                    _response.ErrorMessage = "Coupon Not Found :(";
                    return NotFound(_response);
                }
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
                var couponExists = await _couponService.DeleteCoupon(couponId);
                if (!couponExists)
                {
                    _response.ErrorMessage = "Coupon Not Found :(";
                    return NotFound(_response);
                }
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
