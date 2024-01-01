using AutoMapper;
using CartService.Models;
using CartService.Models.DTOs;

namespace CartService.Profiles
{
    public class CartProfile : Profile
    {
        public CartProfile()
        {
            CreateMap<CartItem, CartItemDTO>();
            CreateMap<ProductImageDTO, CartItemImages>();
        }
    }
}
