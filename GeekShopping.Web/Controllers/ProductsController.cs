using GeekShopping.Web.Models;
using GeekShopping.Web.Services.IServices;
using Microsoft.AspNetCore.Mvc;

namespace GeekShopping.Web.Controllers
{
    public class ProductsController : Controller
    {
        private readonly IProductsService _productsService;

        public ProductsController(IProductsService productsService)
        {
            _productsService = productsService ?? throw new ArgumentNullException(nameof(productsService));
        }

        public async Task<IActionResult> ProductsIndex()
        {
            var products = await _productsService.FindAllProducts();
            return View(products);
        }

        public async Task<IActionResult> ProductsCreate()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ProductsCreate(ProductsModel model)
        {
            if (ModelState.IsValid)
            {
                var response = await _productsService.CreateProduct(model);
                if (response != null) return RedirectToAction(nameof(ProductsIndex));

            }
            return View(model);
        }

        public async Task<IActionResult> ProductsUpdate(int id)
        {
            var productModel = await _productsService.FindProductsById(id);
            if (productModel != null) return View(productModel);
            return NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> ProductsUpdate(ProductsModel model)
        {
            if (ModelState.IsValid)
            {
                var response = await _productsService.UpdateProduct(model);
                if (response != null) return RedirectToAction(nameof(ProductsIndex));

            }
            return View(model);
        }

        public async Task<IActionResult> ProductsDelete(int id)
        {
            var productModel = await _productsService.FindProductsById(id);
            if (productModel != null) return View(productModel);
            return NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> ProductsDelete(ProductsModel model)
        {
            var response = await _productsService.DeleteProductById(model.Id);
            if (response) return RedirectToAction(nameof(ProductsIndex));
            return View(model);
        }
    }
}
