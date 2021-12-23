using GeekShopping.Web.Models;

namespace GeekShopping.Web.Services.IServices
{
    public interface IProductsService
    {
        Task<IEnumerable<ProductsModel>> FindAllProducts();
        Task<ProductsModel> FindProductsById(long id);

        Task<ProductsModel> CreateProduct(ProductsModel model);
        Task<ProductsModel> UpdateProduct(ProductsModel model);
        Task<bool> DeleteProductById(long id);
    }
}
