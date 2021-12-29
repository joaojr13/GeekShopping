using GeekShopping.CartAPI.Models.Base;
using System.ComponentModel.DataAnnotations.Schema;

namespace GeekShopping.CartAPI.Models
{
    [Table("cart_detail")]
    public class CartDetails : BaseEntity
    {
        public long CartHeaderId { get; set; }

        [ForeignKey("CartHeaderId")]
        public virtual CartHeader CartHeader { get; set; }

        public long ProductId { get; set; }

        [ForeignKey("ProductId")]
        public virtual Products Product { get; set; }

        [Column("count")]
        public int Count { get; set; }
    }
}
