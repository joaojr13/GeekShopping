using GeekShopping.OrderAPI.Models;

namespace GeekShopping.CartAPI.Repository
{
    public interface IOrderRepository
    {
        Task<bool> AddOrder(OrderHeader header);
        Task UpdateOrderPaymentStatus(long orderHeaderId, bool paid);
    }
}
