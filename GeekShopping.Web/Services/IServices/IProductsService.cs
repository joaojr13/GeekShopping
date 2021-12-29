using GeekShopping.Web.Models;

namespace GeekShopping.Web.Services.IServices
{
    public interface IProductsService
    {
        Task<IEnumerable<ProductsViewModel>> FindAllProducts(string token);
        Task<ProductsViewModel> FindProductsById(long id, string token);

        Task<ProductsViewModel> CreateProduct(ProductsViewModel model, string token);
        Task<ProductsViewModel> UpdateProduct(ProductsViewModel model, string token);
        Task<bool> DeleteProductById(long id, string token);
    }
}
