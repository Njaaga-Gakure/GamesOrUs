using OrderService.Models;
using OrderService.Models.DTOs;

namespace OrderService.Service.IService
{
    public interface IOrder
    {
        Task<string> AddNewOrder(Order order);
        Task<Order> GetOrderByUserId(Guid UserId);
        Task<StripeRequestDTO> MakePayment(StripeRequestDTO stripeRequest, string token);
        Task<bool> ValidatePayments(Guid orderId, string token, UserDTO user);
    }
}
