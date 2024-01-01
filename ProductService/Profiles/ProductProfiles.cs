using AutoMapper;
using ProductService.Models;
using ProductService.Models.DTOs;

namespace ProductService.Profiles
{
    public class ProductProfiles : Profile
    {
        public ProductProfiles()
        {
            CreateMap<ProductDTO, Product>().ReverseMap();  
            CreateMap<ProductImageDTO, ProductImage>().ReverseMap();
            CreateMap<ProductImage, ImageResponseDTO>().ReverseMap();
        }
    }
}
