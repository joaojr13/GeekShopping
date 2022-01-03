namespace GeekShopping.OrderAPI.Messages
{
    public class CartDetailsVO
    {
        public long Id { get; set; }
        public long CartHeaderId { get; set; }
        public long ProductId { get; set; }
        public virtual ProductsVO Product { get; set; }
        public int Count { get; set; }
    }
}
