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
        private readonly ResponseDTO _response;

        public CartController(ICart cartService, IProduct productService)
        {
            _cartService = cartService;
            _productService = productService;
            _response = new ResponseDTO();
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
                    return Ok(_response);
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
                    await _cartService.RemoveProductFromCart(item.ProductId);
                    var product = await _productService.GetProductById(item.ProductId, token);
                    await _cartService.UpdateCartTotals(cart.CartId, -(cartItem.ProductUnitPrice * cartItem.ProductQuantity));
                    _response.Result = $"{product.Name} has been removed from your cart";
                    return Ok(_response);
                }

                await _cartService.UpdateCartItemQuantity(cartItem.Id, newQuantity);
                await _cartService.UpdateCartTotals(cart.CartId, -(cartItem.ProductUnitPrice * item.ProductQuantity));
                _response.Result = "Item quantity Updated Successfully :)";

                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.ErrorMessage = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                return StatusCode(500, _response);
            }
        }

        [HttpDelete("{productId}")]
        [Authorize]
        public async Task<ActionResult<ResponseDTO>> RemoveItemFromCart(Guid productId)
        {
            var token = HttpContext.Request.Headers["Authorization"].ToString().Split(" ")[1];
            var isProductRemoved = await _cartService.RemoveProductFromCart(productId);
            if (!isProductRemoved)
            {
                _response.ErrorMessage = "The product you are trying to remove does not exist in your cart :(";
                return NotFound(_response);
            }
            var product = await _productService.GetProductById(productId, token);
            _response.Result = $"{product.Name} has been removed from your cart";
            return Ok(_response);
        }
    }
}
