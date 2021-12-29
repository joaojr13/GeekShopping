namespace GeekShopping.Web.Models
{
    public class CartDetailsViewModel
    {
        public long Id { get; set; }
        public long CartHeaderId { get; set; }
        public CartHeaderViewModel CartHeader { get; set; }
        public long ProductId { get; set; }
        public ProductsViewModel Product { get; set; }
        public int Count { get; set; }
    }
}
