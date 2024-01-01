using AutoMapper;
using CartService.Models;
using CartService.Models.DTOs;
using CartService.Service.IService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CartService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartController : ControllerBase
    {
        private readonly ICart _cartService;
        private readonly IProduct _productService;
        private readonly ICoupon _couponService;
        private readonly ResponseDTO _response;
        private readonly IMapper _mapper;

        public CartController(ICart cartService, IProduct productService, ICoupon couponService, IMapper mapper)
        {
            _cartService = cartService;
            _productService = productService;
            _couponService = couponService;
            _response = new ResponseDTO();
            _mapper = mapper;   
        }

        [HttpPost]
        [Authorize]
        public async Task<ActionResult<ResponseDTO>> AddProductToCart(AddToCartDTO item)
        {
            try
            {
                var token = HttpContext.Request.Headers["Authorization"].ToString().Split(" ")[1];
                var userId = User.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.NameIdentifier)?.Value;
                if (userId == null)
                {
                    _response.ErrorMessage = "You are not authorized";
                    return StatusCode(403, _response);
                }

                // check if product exists 
                var product = await _productService.GetProductById(item.ProductId, token);
                if (product == null)
                {
                    _response.ErrorMessage = "The product you are trying to add does not exist :(";
                    return NotFound(_response);
                }

                if (item.ProductQuantity > product.Stock)
                {
                    _response.ErrorMessage = "The quantity you want exceeds the available stock:(";
                    return BadRequest(_response);
                }

                // check if user has already added a product to cart
                var cart = await _cartService.GetCartByUserId(new Guid(userId));
                if (cart == null)
                {
                    var newCart = new Cart();
                    newCart.UserId = new Guid(userId);
                    await _cartService.CreateCart(newCart);
                    cart = await _cartService.GetCartByUserId(new Guid(userId));

                }
                var cartProduct = cart.CartItems.Find(cartI => cartI.ProductId == product.Id);

                // check if it's a new product or a product existing in the cart
                if (cartProduct == null)
                {
                    if (item.ProductQuantity == 0)
                    {
                        _response.ErrorMessage = "You must at least add one product to the cart :(";
                        return BadRequest(_response);
                    }
                    var cartItem = new CartItem()
                    {
                        ProductId = product.Id,
                        ProductName = product.Name,
                        ProductDescription = product.Description,
                        ProductGenre = product.Genre,
                        ProductUnitPrice = product.Price,
                        ProductQuantity = item.ProductQuantity,
                        ProductImages = _mapper.Map<List<CartItemImages>>(product.ProductImages),
                        CartId = cart.CartId
                    };
                    await _cartService.AddToCart(cartItem);
                }
                else
                {
                    var newQuantity = cartProduct.ProductQuantity + item.ProductQuantity;
                    if (newQuantity > product.Stock)
                    {
                        _response.ErrorMessage = "The quantity you want exceeds the available stock:(";
                        return BadRequest(_response);
                    }
                    await _cartService.UpdateCartItemQuantity(cartProduct.Id, newQuantity);
                }

                await _cartService.UpdateCartTotals(cart.CartId, (product.Price * item.ProductQuantity));
                _response.Result = "Product Added To Cart Successfully :)";
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
        public async Task<ActionResult<ResponseDTO>> ViewCart()
        {
            try
            {
                var userId = User.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.NameIdentifier)?.Value;
                if (userId == null)
                {
                    _response.ErrorMessage = "You are not authorized";
                    return StatusCode(403, _response);
                }

                var cart = await _cartService.GetCartByUserId(new Guid(userId));

                if (cart == null || cart.CartItems.Count == 0)
                {
                    _response.Result = "Your Cart is Empty :(";
                    return Ok(_response);
                }
                _response.Result = cart;
                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.ErrorMessage = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                return StatusCode(500, _response);
            }
        }

        // for external services
        [HttpGet("{userId}")]
        [Authorize]
        public async Task<ActionResult<ResponseDTO>> GetCartByUserId(Guid userId)
        {
            try
            {
                var cart = await _cartService.GetCartByUserId(userId);

                if (cart == null || cart.CartItems.Count == 0)
                {
                    _response.Result = "Your Cart is Empty :(";
                    return Ok(_response);
                }
                _response.Result = cart;
                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.ErrorMessage = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                return StatusCode(500, _response);
            }
        }


        // To do: If user had a coupon applied and reduces a product's quantity from cart
        // add the the total amount is below the coupon minimum amount
        // remove the applied coupon
        // Edit: done the above in second draft
        [HttpPatch]
        public async Task<ActionResult<ResponseDTO>> ReduceCartItemQuantity(AddToCartDTO item)
        {
            try
            {
                var token = HttpContext.Request.Headers["Authorization"].ToString().Split(" ")[1];
                var userId = User.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.NameIdentifier)?.Value;
                if (userId == null)
                {
                    _response.ErrorMessage = "You are not authorized";
                    return StatusCode(403, _response);
                }

                var cart = await _cartService.GetCartByUserId(new Guid(userId));
                if (cart == null || cart.CartItems.Count == 0)
                {
                    _response.Result = "Your Cart is Empty :(";
                    return BadRequest(_response);
                }
                var cartItem = cart.CartItems.Find(cartItem => cartItem.ProductId == item.ProductId);

                if (cartItem == null)
                {
                    _response.ErrorMessage = "The product is not in your cart :(";
                    return NotFound(_response);
                }

                var newQuantity = cartItem.ProductQuantity - item.ProductQuantity;

                if (newQuantity <= 0)
                {
                    // check this error
                    // Edit: found error. Needed to pass token to http client
                    await _cartService.RemoveProductFromCart(item.ProductId);
                    var totalPriceToRemove = cartItem.ProductUnitPrice * cartItem.ProductQuantity;
                    var product = await _productService.GetProductById(item.ProductId, token);
                    await _cartService.UpdateCartTotals(cart.CartId, -totalPriceToRemove);
                    if (!string.IsNullOrWhiteSpace(cart.CouponCode))
                    {
                        var coupon = await _couponService.GetCouponByCouponCode(cart.CouponCode, token);
                        if ((cart.TotalAmount - totalPriceToRemove) < coupon.CouponMinimumAmount)
                        {
                            await _cartService.UpdateCartCouponDetails(cart.CartId, 0, "");
                        }
                    }
                    _response.Result = $"{product.Name} has been removed from your cart";
                    return Ok(_response);
                }

                await _cartService.UpdateCartItemQuantity(cartItem.Id, newQuantity);
                var totalPriceRemoved = cartItem.ProductUnitPrice * item.ProductQuantity;
                await _cartService.UpdateCartTotals(cart.CartId, -totalPriceRemoved);
                if (cart.CouponCode != null)
                {
                    var coupon = await _couponService.GetCouponByCouponCode(cart.CouponCode, token);
                    if ((cart.TotalAmount - totalPriceRemoved) < coupon.CouponMinimumAmount)
                    {
                        await _cartService.UpdateCartCouponDetails(cart.CartId, 0, "");
                    }
                }
                _response.Result = "Item quantity Updated Successfully :)";

                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.ErrorMessage = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                return StatusCode(500, _response);
            }
        }
        [HttpPatch("{couponCode}")]
        [Authorize]
        public async Task<ActionResult<ResponseDTO>> ApplyCoupon(string couponCode)
        {
            try
            {
                var userId = User.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.NameIdentifier)?.Value;
                if (userId == null)
                {
                    _response.ErrorMessage = "You are not authorized";
                    return StatusCode(403, _response);
                }
                // get token from auth headers
                var token = HttpContext.Request.Headers["Authorization"].ToString().Split(" ")[1];
                // check if coupon exists
                var coupon = await _couponService.GetCouponByCouponCode(couponCode, token);

                if (coupon == null)
                {
                    _response.ErrorMessage = "Coupon Not Found";
                    return NotFound(_response);
                }

                var cart = await _cartService.GetCartByUserId(new Guid(userId));

                if (cart == null)
                {
                    _response.ErrorMessage = "Your Cart is Empty :(";
                    return BadRequest(_response);
                }

                if (!string.IsNullOrWhiteSpace(cart.CouponCode))
                {
                    _response.ErrorMessage = "You already have a coupon applied :(";
                    return BadRequest(_response);
                }

                if (cart.TotalAmount < coupon.CouponMinimumAmount)
                {
                    _response.ErrorMessage = $"Purchase games worth Ksh {coupon.CouponMinimumAmount - cart.TotalAmount} more to apply this coupon :(";
                    return BadRequest(_response);
                }

                await _cartService.UpdateCartCouponDetails(cart.CartId, coupon.CouponDiscount, coupon.CouponCode);
                _response.Result = "Coupon Applied Successfully :)";
                return Ok(_response);

            }
            catch (Exception ex)
            {
                _response.ErrorMessage = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                return StatusCode(500, _response);
            }


        }

        // To do: If user had a coupon applied` and removed a product from cart
        // add the the total amount is below the coupon minimum amount
        // remove the applied coupon
        // Edit: done the above in second draft

        [HttpDelete("{productId}")]
        [Authorize]
        public async Task<ActionResult<ResponseDTO>> RemoveItemFromCart(Guid productId)
        {
            var token = HttpContext.Request.Headers["Authorization"].ToString().Split(" ")[1];
            var userId = User.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                _response.ErrorMessage = "You are not authorized";
                return StatusCode(403, _response);
            }

            var cart = await _cartService.GetCartByUserId(new Guid(userId));
            if (cart == null || cart.CartItems.Count == 0)
            {
                _response.Result = "Your Cart is Empty :(";
                return BadRequest(_response);
            }

            var cartItem = cart.CartItems.Find(cartItem => cartItem.ProductId == productId);
            if (cartItem == null)
            {
                _response.ErrorMessage = "The product is not in your cart :(";
                return NotFound(_response);
            }
            var totalAmountRemoved = cartItem.ProductUnitPrice * cartItem.ProductQuantity;
            await _cartService.RemoveProductFromCart(productId);
            await _cartService.UpdateCartTotals(cart.CartId, -(totalAmountRemoved));
            if (!string.IsNullOrWhiteSpace(cart.CouponCode))
            {
                var coupon = await _couponService.GetCouponByCouponCode(cart.CouponCode, token);
                cart = await _cartService.GetCartByUserId(new Guid(userId));
                if (cart.TotalAmount < coupon.CouponMinimumAmount)
                {
                    await _cartService.UpdateCartCouponDetails(cart.CartId, 0, "");
                }
            }
            var product = await _productService.GetProductById(productId, token);
            _response.Result = $"{product.Name} has been removed from your cart";
            return Ok(_response);
        }
    }
}
