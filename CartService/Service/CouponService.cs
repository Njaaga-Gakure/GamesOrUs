using CartService.Models.DTOs;
using CartService.Service.IService;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Net.Http.Headers;

namespace CartService.Service
{
    public class CouponService : ICoupon
    {

        private readonly IHttpClientFactory _httpClientFactory;

        public CouponService(IHttpClientFactory clientFactory)
        {
            _httpClientFactory = clientFactory;
        }
        public async Task<CouponDTO> GetCouponByCouponCode(string couponCode, string token)
        {
            var client = _httpClientFactory.CreateClient("Coupon");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await client.GetAsync(couponCode);
            var content = await response.Content.ReadAsStringAsync();
            var responseDTO = JsonConvert.DeserializeObject<ResponseDTO>(content);

            if (response.IsSuccessStatusCode)
            {
                var coupon = JsonConvert.DeserializeObject<CouponDTO>(responseDTO.Result.ToString());
                return coupon;
            }
            return null;
        }
    }
}
