using AutoMapper;
using Microsoft.EntityFrameworkCore;
using ProductService.Data;
using ProductService.Models;
using ProductService.Models.DTOs;
using ProductService.Service.IService;

namespace ProductService.Service
{
    public class ProductsService : IProduct
    {
        private readonly ProductContext _context;
        private readonly IMapper _mapper;

        public ProductsService(ProductContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;   
        }
        public async Task<string> AddProduct(Product product)
        {
            _context.Products.Add(product);
            await _context.SaveChangesAsync();
            return "Product Added Successfully :)";
        }


        public async Task<List<ProductResponseDTO>> GetAllProducts()
        {
            var products = await _context.Products
                                .Include(product => product.ProductImages).
                                 Select(product => new ProductResponseDTO() {
                                     Id = product.Id,
                                     Name = product.Name,
                                     Description = product.Description,
                                     Genre = product.Genre,
                                     Stock = product.Stock,
                                     Price = product.Price,
                                     ProductImages = _mapper.Map<List<ImageResponseDTO>>(product.ProductImages.ToList())
                                 })
                                 .ToListAsync();
            return products;
        }

        public async Task<ProductResponseDTO> GetProductById(Guid productId)
        {
            var product = await _context.Products
                            .Where(product => product.Id == productId)
                            .Select(product => new ProductResponseDTO()
                             {
                                 Id = product.Id,
                                 Name = product.Name,
                                 Description = product.Description,
                                 Genre = product.Genre,
                                 Stock = product.Stock,
                                 Price = product.Price,
                                 ProductImages = _mapper.Map<List<ImageResponseDTO>>(product.ProductImages.ToList())
                             })
                            .FirstOrDefaultAsync();
            return product;
        }

        public async Task<bool> UpdateProduct(Guid productId, ProductDTO updateProduct)
        {
            var product = await _context.Products.Where(product => product.Id == productId).FirstOrDefaultAsync();
            if (product != null)
            {
                product.Name = updateProduct.Name;
                product.Description = updateProduct.Description;
                product.Genre = updateProduct.Genre;
                product.Stock = updateProduct.Stock;
                product.Price = updateProduct.Price;
                await _context.SaveChangesAsync();
                return true;
            }
            return false;
        }
        public async Task<bool> DeleteProduct(Guid productId)
        {
            var product = await _context.Products.Where(product => product.Id == productId).FirstOrDefaultAsync();
            if (product != null)
            {
                _context.Products.Remove(product);
                await _context.SaveChangesAsync();
                return true;
            }
            return false;
        }
    }
}
