using Newtonsoft.Json;
using OrderService.Models.DTOs;
using OrderService.Service.IService;
using System.Net.Http.Headers;

namespace OrderService.Service
{
    public class CartService : ICart
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public CartService(IHttpClientFactory clientFactory)
        {
            _httpClientFactory = clientFactory;
        }
        public async Task<CartDTO> GetCartByUserId(Guid UserId, string token)
        {
            var client = _httpClientFactory.CreateClient("Cart");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await client.GetAsync($"{UserId}");
            var content = await response.Content.ReadAsStringAsync();
            var responseDTO = JsonConvert.DeserializeObject<ResponseDTO>(content);

            if (response.IsSuccessStatusCode)
            {
                var cart = JsonConvert.DeserializeObject<CartDTO>(responseDTO.Result.ToString());
                return cart;
            }
            return null;
        }

        public async Task<bool> DeleteCart(Guid UserId, string token)
        {
            var cart = await GetCartByUserId(UserId, token);
            var client = _httpClientFactory.CreateClient("Cart");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await client.DeleteAsync($"{cart.Id}");
            if (response.IsSuccessStatusCode)
            {
                return true;
            }
            return false;
        }

    }
}
