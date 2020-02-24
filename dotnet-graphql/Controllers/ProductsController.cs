using System.Collections.Generic;
using System.Threading.Tasks;
using dotnet_graphql.Models;
using dotnet_graphql.Services;
using Microsoft.AspNetCore.Mvc;
using ExpressMapper;

namespace dotnet_graphql.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly ProductService _productService;

        public ProductsController(ProductService productService)
        {
            _productService = productService;
        }

        //// GET api/products
        /// <summary>
        /// Get products.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductViewModel>>> Get()
        {
            var products = await _productService.GetProducts();
            var productViewModels = Mapper.Map<IEnumerable<Product>, IEnumerable<ProductViewModel>>(products);
            return new ObjectResult(productViewModels);
        }

        /// GET api/products/1
        /// <summary>
        /// Get Product by Id.
        /// </summary>
        /// <param name="id">product id.</param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<ProductViewModel>> Get(int id)
        {
            var product = await _productService.GetProduct(id);
            if (product == null)
                return new NotFoundResult();

            var productViewModel = Mapper.Map<Product, ProductViewModel>(product);
            return new OkObjectResult(productViewModel);
        }

        /// GET api/products/{id}/sizes
        /// <summary>
        /// Get sizes of product
        /// </summary>
        /// <param name="id">product id.</param>
        /// <returns></returns>
        [Route("{id:int}/sizes")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Size>>> GetSizesOfProduct(int id)
        {
            object sizes = await _productService.GetSizeOfProduct(id);

            if (sizes == null)
                return new NotFoundResult();

            return new OkObjectResult(sizes);
        }

        /// POST api/products
        /// <summary>
        /// Create product
        /// </summary>
        /// <param name="productViewModel">product view model.</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult<ProductViewModel>> Post([FromBody] ProductViewModel productViewModel)
        {
            var product = await _productService.Create(productViewModel);
            var productView = Mapper.Map<Product, ProductViewModel>(product);
            return new OkObjectResult(productView);
        }

        /// PUT api/products/1
        /// <summary>
        /// Update Product.
        /// </summary>
        /// <param name="productViewModel">product view model.</param>
        /// <returns></returns>
        [HttpPut("{id}")]
        public async Task<ActionResult<ProductViewModel>> Put([FromBody] ProductViewModel productViewModel)
        {
            var product = await _productService.UpdateProductAsync(productViewModel);

            if (product == null)
                return new NotFoundResult();

            var productView = Mapper.Map<Product, ProductViewModel>(product);

            return new OkObjectResult(productView);
        }

        [HttpDelete("{id}")]
        public async Task<bool> Delete(int id)
        {
            return await _productService.DeleteProductAsync(id);
        }
    }
}
