using GamesOrUsMessageBus;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OrderService.Models;
using OrderService.Models.DTOs;
using OrderService.Service.IService;
using System.ComponentModel.DataAnnotations.Schema;
using System.Security.Claims;

namespace OrderService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IOrder _orderService;
        private readonly ICart _cartService;
        private readonly ResponseDTO _response;
        public OrderController(IOrder orderService, ICart cartService)
        {
            _orderService = orderService;
            _cartService = cartService;
            _response = new ResponseDTO();
        }

        [HttpPost]
        [Authorize]
        public async Task<ActionResult<ResponseDTO>> AddOrder()
        {
            try
            {
                var userId = User.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.NameIdentifier)?.Value;
                var token = HttpContext.Request.Headers["Authorization"].ToString().Split(" ")[1];
                if (userId == null)
                {
                    _response.ErrorMessage = "You are not authorized";
                    return StatusCode(500, _response);
                }
                var cart = await _cartService.GetCartByUserId(new Guid(userId), token);

                if (cart == null || cart.TotalAmount == 0)
                {
                    _response.ErrorMessage = "Your cart is empty :(";
                    return BadRequest(_response);
                }

                var order = new Order()
                {
                    UserId = new Guid(userId),
                    TotalAmount = cart.TotalAmount,
                    CouponCode = cart.CouponCode,
                    Discount = cart.Discount
                };

                var response = await _orderService.AddNewOrder(order);
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
        public async Task<ActionResult<ResponseDTO>> GetOrderUserId()
        {
            try
            {
                var userId = User.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.NameIdentifier)?.Value;
                if (userId == null)
                {
                    _response.ErrorMessage = "You are not authorized";
                    return StatusCode(500, _response);
                }
                var order = await _orderService.GetOrderByUserId(new Guid(userId));

                if (order == null)
                {
                    _response.ErrorMessage = "You have not placed an order yet :(";
                    return NotFound(_response);
                }

                _response.Result = order;
                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.ErrorMessage = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                return StatusCode(500, _response);
            }

        }

        [HttpPost("payment")]
        [Authorize]
        public async Task<ActionResult<ResponseDTO>> MakePayment(StripeRequestDTO stripeRequest)
        {
            try
            {
                var token = HttpContext.Request.Headers["Authorization"].ToString().Split(" ")[1];
                if (token == null)
                {
                    _response.ErrorMessage = "You are not authorized";
                    return StatusCode(500, _response);
                }

                var stripeRequestDTO = await _orderService.MakePayment(stripeRequest, token);
                _response.Result = stripeRequestDTO;
                return Ok(_response);

            }
            catch (Exception ex)
            {
                _response.ErrorMessage = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                return StatusCode(500, _response);
            }

        }

        [HttpPost("validatePayment/{orderId}")]
        [Authorize]
        public async Task<ActionResult<ResponseDTO>> ValidatePayment(Guid orderId)
        {
            try
            {
                var token = HttpContext.Request.Headers["Authorization"].ToString().Split(" ")[1];
                var name = User.Claims.FirstOrDefault(claim => claim.Type == "name")?.Value;
                var email = User.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.Email)?.Value;
                if (token == null || name == null || email == null)
                {
                    _response.ErrorMessage = "You are not authorized";
                    return StatusCode(500, _response);
                }
                var user = new UserDTO() { Name = name, Email = email };
                var response = await _orderService.ValidatePayments(orderId, token, user);

                if (response)
                {
                    _response.Result = "Payment Was Successfull :)";
                    return Ok(_response);

                }
                _response.ErrorMessage = "Payment Failed :(";
                return BadRequest(_response);
            }
            catch (Exception ex)
            {
                _response.ErrorMessage = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                return StatusCode(500, _response);
            }

        }

    }
}
