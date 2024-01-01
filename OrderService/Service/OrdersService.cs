using Microsoft.EntityFrameworkCore;
using OrderService.Data;
using OrderService.Models;
using OrderService.Models.DTOs;
using OrderService.Service.IService;
using Stripe;
using Stripe.Checkout;
using Stripe.Issuing;

namespace OrderService.Service
{
    public class OrdersService : IOrder
    {

        private readonly OrderContext _context;
        private readonly ICart _cartService;

        public OrdersService(OrderContext context, ICart cartService)
        {
            _context = context;
            _cartService = cartService; 
        }
        public async Task<string> AddNewOrder(Order order)
        {
            await _context.Orders.AddAsync(order);
            await _context.SaveChangesAsync();
            return "Order Added Successfully :)";
        }

        public async Task<Order> GetOrderByUserId(Guid UserId)
        {
            var order = await _context.Orders.Where(order => order.UserId == UserId).FirstOrDefaultAsync();
            return order;

        }

        public async Task<StripeRequestDTO> MakePayment(StripeRequestDTO stripeRequest, string token)
        {
            var order = await _context.Orders.Where(order => order.Id == stripeRequest.OrderId).FirstOrDefaultAsync();
            var cart = await _cartService.GetCartByUserId(order.UserId, token);

            var options = new SessionCreateOptions()
            {
                SuccessUrl = stripeRequest.ApprovedURL,
                CancelUrl = stripeRequest.CancelURL,
                Mode = "payment",
                LineItems = new List<SessionLineItemOptions>()
            };


            foreach (var cartItem in cart.CartItems)
            {
                var item = new SessionLineItemOptions()
                {
                    PriceData = new SessionLineItemPriceDataOptions()
                    {
                        UnitAmount = (long) cartItem.ProductUnitPrice * 100,
                        Currency = "kes",

                        // Second Draft: Got Image from Cart Item
                        // and not just used a random image
                        ProductData = new SessionLineItemPriceDataProductDataOptions()
                        {
                            Name = cartItem.ProductName,
                            Description = cartItem.ProductDescription,
                            Images = new List<string> { cartItem.ProductImage }
                        }
                    },
                    Quantity = cartItem.ProductQuantity


                };

                options.LineItems.Add(item);
            }
           

            var DiscountObject = new List<SessionDiscountOptions>()
            {
                new SessionDiscountOptions()
                {
                    Coupon=order.CouponCode
                }
            };

            if (order.Discount > 0)
            {
                options.Discounts = DiscountObject;

            }

            var service = new SessionService();
            Session session = service.Create(options);

          

            stripeRequest.StripeSessionURL = session.Url;
            stripeRequest.StripeSessionId = session.Id;


            order.StripeSessionId = session.Id;
            order.Status = "Ongoing";
            await _context.SaveChangesAsync();

            return stripeRequest;
        }

        public async Task<bool> ValidatePayments(Guid orderId, string token, UserDTO user)
        {
            var order = await _context.Orders.Where(order => order.Id == orderId).FirstOrDefaultAsync();
            var cart = await _cartService.GetCartByUserId(order.UserId, token);

            var service = new SessionService();
            Session session = service.Get(order.StripeSessionId);

            PaymentIntentService paymentIntentService = new PaymentIntentService();

            PaymentIntent paymentIntent = paymentIntentService.Get(session.PaymentIntentId);

            if (paymentIntent.Status == "succeeded")
            {
                order.Status = "Paid";
                order.PaymentIntent = paymentIntent.Id;
                await _context.SaveChangesAsync();
                await _cartService.DeleteCart(cart.Id, token);
                return true;

            }
            return false;
        }
    }
}
