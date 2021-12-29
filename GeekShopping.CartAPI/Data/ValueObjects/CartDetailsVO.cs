namespace GeekShopping.CartAPI.Data.ValueObjects
{
    public class CartDetailsVO
    {
        public long Id { get; set; }
        public long CartHeaderId { get; set; }
        public CartHeaderVO CartHeader { get; set; }
        public long ProductId { get; set; }
        public ProductsVO Product { get; set; }
        public int Count { get; set; }
    }
}
