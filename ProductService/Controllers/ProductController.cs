using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProductService.Models;
using ProductService.Models.DTOs;
using ProductService.Service.IService;

namespace ProductService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IProduct _productService;
        private readonly ResponseDTO _response;

        public ProductController(IMapper mapper, IProduct productService)
        {
            _mapper = mapper;
            _productService = productService;
            _response = new ResponseDTO();
        }

        [HttpPost]
        [Authorize]
        public async Task<ActionResult<ResponseDTO>> AddProduct(ProductDTO newProduct)
        {
            try
            {
                var product = _mapper.Map<Product>(newProduct);
                var response = await _productService.AddProduct(product);
                _response.Result = response;
                return Created($"api/v1/Product/{product.Id}", _response);
            }
            catch (Exception ex)
            {
                _response.ErrorMessage = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                return StatusCode(500, _response);
            }
        }

        // Note to self: if GET requests are authorized, cannot make request from another service
        // ask about this
        [HttpGet]
        public async Task<ActionResult<ResponseDTO>> GetAllProducts()
        {
            try
            {
                var products = await _productService.GetAllProducts();
                _response.Result = products;
                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.ErrorMessage = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                return StatusCode(500, _response);
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ResponseDTO>> GetSingleProduct(Guid id)
        {
            try
            {
                var product = await _productService.GetProductById(id);
                if (product == null)
                {
                    _response.ErrorMessage = "Product Not Found :(";
                    return NotFound(_response);
                }
                _response.Result = product;
                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.ErrorMessage = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                return StatusCode(500, _response);
            }
        }

        [HttpPatch("{id}")]
        [Authorize]
        public async Task<ActionResult<ResponseDTO>> UpdateProduct(Guid id, ProductDTO updateProduct)
        {
            try
            {
                var productExists= await _productService.UpdateProduct(id, updateProduct);
                if (!productExists)
                {
                    _response.ErrorMessage = "Product Not Found :(";
                    return NotFound(_response);
                }
                _response.Result = "Product Updated Successfully :)";
                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.ErrorMessage = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                return StatusCode(500, _response);
            }
        }

        [HttpDelete]
        [Authorize]
        public async Task<ActionResult<ResponseDTO>> DeleteProduct(Guid id)
        {
            try
            {
                var productExists = await _productService.DeleteProduct(id);
                if (!productExists)
                {
                    _response.ErrorMessage = "Product Not Found :(";
                    return NotFound(_response);
                }
                _response.Result = "Product Deleted Successfully :)";
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
