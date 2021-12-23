using GeekShopping.Web.Models;
using GeekShopping.Web.Utils;

namespace GeekShopping.Web.Services.IServices
{
    public class ProductsService : IProductsService
    {
        private readonly HttpClient _httpClient;
        public const string BasePath = "Products";

        public ProductsService(HttpClient client)
        {
            _httpClient = client ?? throw new ArgumentNullException(nameof(client));
        }

        public async Task<IEnumerable<ProductsModel>> FindAllProducts()
        {
            var response = await _httpClient.GetAsync(BasePath);
            return await response.ReadContentAsync<List<ProductsModel>>();
        }

        public async Task<ProductsModel> FindProductsById(long id)
        {
            var response = await _httpClient.GetAsync($"{BasePath}/{id}");
            return await response.ReadContentAsync<ProductsModel>();
        }

        public async Task<ProductsModel> CreateProduct(ProductsModel model)
        {
            var response = await _httpClient.PostAsJson(BasePath, model);
            if (!response.IsSuccessStatusCode) throw new Exception("Something went wrong when calling API");
            return await response.ReadContentAsync<ProductsModel>();
        }

        public async Task<ProductsModel> UpdateProduct(ProductsModel model)
        {
            var response = await _httpClient.PutAsJson(BasePath, model);
            if (!response.IsSuccessStatusCode) throw new Exception($"Something went wrong when calling API");
            return await response.ReadContentAsync<ProductsModel>();
        }

        public async Task<bool> DeleteProductById(long id)
        {
            var response = await _httpClient.DeleteAsync($"{BasePath}/{id}");
            if(!response.IsSuccessStatusCode) throw new Exception("Something went wrong when calling API");
            return await response.ReadContentAsync<bool>();
        }

    }
}
