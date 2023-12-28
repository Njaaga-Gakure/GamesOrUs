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

        public ProductsService(ProductContext context)
        {
           _context = context;  
        }
        public async Task<string> AddProduct(Product product)
        {
           _context.Products.Add(product);
           await _context.SaveChangesAsync();
            return "Product Added Successfully :)"; 
        }


        public async Task<List<Product>> GetAllProducts()
        {
            var products = await _context.Products.ToListAsync();
            return products;    
        }

        public async Task<Product> GetProductById(Guid productId)
        {
            var product = await _context.Products.Where(product => product.Id == productId).FirstOrDefaultAsync();
            return product;    
        }

        public async Task<bool> UpdateProduct(Guid productId, ProductDTO updateProduct)
        {
           var product = await GetProductById(productId);
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
            var product = await GetProductById(productId);
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
