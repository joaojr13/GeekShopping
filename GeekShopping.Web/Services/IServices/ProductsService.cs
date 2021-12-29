using GeekShopping.Web.Models;
using GeekShopping.Web.Utils;
using System.Net.Http.Headers;

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

        public async Task<IEnumerable<ProductsViewModel>> FindAllProducts(string token)
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await _httpClient.GetAsync(BasePath);
            return await response.ReadContentAsync<List<ProductsViewModel>>();
        }

        public async Task<ProductsViewModel> FindProductsById(long id, string token)
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await _httpClient.GetAsync($"{BasePath}/{id}");
            return await response.ReadContentAsync<ProductsViewModel>();
        }

        public async Task<ProductsViewModel> CreateProduct(ProductsViewModel model, string token)
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await _httpClient.PostAsJson(BasePath, model);
            if (!response.IsSuccessStatusCode) throw new Exception("Something went wrong when calling API");
            return await response.ReadContentAsync<ProductsViewModel>();
        }

        public async Task<ProductsViewModel> UpdateProduct(ProductsViewModel model, string token)
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await _httpClient.PutAsJson(BasePath, model);
            if (!response.IsSuccessStatusCode) throw new Exception($"Something went wrong when calling API");
            return await response.ReadContentAsync<ProductsViewModel>();
        }

        public async Task<bool> DeleteProductById(long id, string token)
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await _httpClient.DeleteAsync($"{BasePath}/{id}");
            if(!response.IsSuccessStatusCode) throw new Exception("Something went wrong when calling API");
            return await response.ReadContentAsync<bool>();
        }

    }
}
