using GeekShopping.Web.Models;
using GeekShopping.Web.Services.IServices;
using GeekShopping.Web.Utils;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
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

        [Authorize]
        public async Task<IActionResult> ProductsIndex()
        {
            var token = await HttpContext.GetTokenAsync("access_token");
            var products = await _productsService.FindAllProducts(token);
            return View(products);
        }

        public async Task<IActionResult> ProductsCreate()
        {
            return View();
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> ProductsCreate(ProductsViewModel model)
        {
            if (ModelState.IsValid)
            {
                var token = await HttpContext.GetTokenAsync("access_token");
                var response = await _productsService.CreateProduct(model, token);
                if (response != null) return RedirectToAction(nameof(ProductsIndex));

            }
            return View(model);
        }

        public async Task<IActionResult> ProductsUpdate(int id)
        {
            var token = await HttpContext.GetTokenAsync("access_token");
            var productModel = await _productsService.FindProductsById(id, token);
            if (productModel != null) return View(productModel);
            return NotFound();
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> ProductsUpdate(ProductsViewModel model)
        {
            if (ModelState.IsValid)
            {
                var token = await HttpContext.GetTokenAsync("access_token");
                var response = await _productsService.UpdateProduct(model, token);
                if (response != null) return RedirectToAction(nameof(ProductsIndex));

            }
            return View(model);
        }

        [Authorize]
        public async Task<IActionResult> ProductsDelete(int id)
        {
            var token = await HttpContext.GetTokenAsync("access_token");
            var productModel = await _productsService.FindProductsById(id, token);
            if (productModel != null) return View(productModel);
            return NotFound();
        }

        [Authorize(Roles = Role.Admin)]
        [HttpPost]
        public async Task<IActionResult> ProductsDelete(ProductsViewModel model)
        {
            var token = await HttpContext.GetTokenAsync("access_token");
            var response = await _productsService.DeleteProductById(model.Id, token);
            if (response) return RedirectToAction(nameof(ProductsIndex));
            return View(model);
        }
    }
}
