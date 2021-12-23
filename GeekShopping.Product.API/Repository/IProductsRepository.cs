using GeekShopping.Product.API.Data.ValueObjects;

namespace GeekShopping.Product.API.Repository
{
    public interface IProductsRepository
    {
        Task<IEnumerable<ProductsVO>> FindAll();
        Task<ProductsVO> FindById(long id);
        Task<ProductsVO> Create(ProductsVO vo);
        Task<ProductsVO> Update(ProductsVO vo);
        Task<bool> Delete(long id);
    }
}
