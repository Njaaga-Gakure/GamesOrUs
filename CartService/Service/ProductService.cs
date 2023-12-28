using CartService.Models.DTOs;
using CartService.Service.IService;
using Newtonsoft.Json;

namespace CartService.Service
{
    public class ProductService : IProduct
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public ProductService(IHttpClientFactory clientFactory)
        {
            _httpClientFactory = clientFactory;
        }
        public async Task<ProductDTO> GetProductById(Guid productId)
        {
            var client = _httpClientFactory.CreateClient("Product");
            var response = await client.GetAsync($"{productId}");
            var content = await response.Content.ReadAsStringAsync();
            var responseDTO = JsonConvert.DeserializeObject<ResponseDTO>(content);

            if (response.IsSuccessStatusCode)
            {
                var product = JsonConvert.DeserializeObject<ProductDTO>(responseDTO.Result.ToString());
                return product;
            }
            return null;
        }
    }
}
