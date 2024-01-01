using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProductService.Models;
using ProductService.Models.DTOs;
using ProductService.Service.IService;

namespace ProductService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductImageController : ControllerBase
    {
        private readonly IProductImage _productImageService;
        private readonly IProduct _productService;
        private readonly IMapper _mapper;
        private readonly ResponseDTO _response;


        public ProductImageController(IProductImage productImageService, IProduct productService, IMapper mapper)
        {
            _productImageService = productImageService;
            _productService = productService;
            _mapper = mapper;
            _response = new ResponseDTO();
        }

        // Second Draft: added images to products
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ResponseDTO>> AddProductImage(ProductImageDTO image)
        {
            try
            {
                var product = await _productService.GetProductById(image.ProductId);

                if (product == null) 
                {
                    _response.ErrorMessage = "Product Does Not Exist :(";
                    return NotFound(_response);
                }
                var productImage = _mapper.Map<ProductImage>(image);

                var response = await _productImageService.AddProductImage(productImage);

                _response.Result = response;
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
