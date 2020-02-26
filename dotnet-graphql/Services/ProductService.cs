using dotnet_graphql.Models;
using dotnet_graphql.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using ExpressMapper;


namespace dotnet_graphql.Services
{
    public class ProductService
    {
        private readonly AppDbContext _appDbContext;

        public ProductService(AppDbContext context)
        {
            _appDbContext = context ?? throw new ArgumentNullException(nameof(context));
        }

        /// <summary>
        /// Get all products.
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<Product>> GetProducts()
        {
            return await _appDbContext.Products.Include(p => p.Sizes)
                .Include(p => p.ProductType).Include(p => p.ProductBrand)
                .AsNoTracking().ToListAsync();
        }

        /// <summary>
        /// Get product.
        /// </summary>
        /// <param name="id">product Id.</param>
        /// <returns></returns>
        public async Task<Product> GetProduct(int id)
        {
            return await _appDbContext.Products.Include(p => p.Sizes)
                .Include(a => a.ProductType).Include(a => a.ProductBrand).AsNoTracking()
                .SingleOrDefaultAsync(p => p.Id == id);
        }

        /// <summary>
        /// Get sizes of product.
        /// </summary>
        /// <param name="id">product Id.</param>
        /// <returns></returns>
        public async Task<IEnumerable<Size>> GetSizeOfProduct(int id)
        {
            return await _appDbContext.Sizes.Where(p => p.ProductId == id).ToListAsync();
        }

        /// <summary>
        /// Create product.
        /// </summary>
        /// <param name="productViewModel">product view model.</param>
        /// <returns></returns>
        public async Task<Product> Create(ProductViewModel productViewModel)
        {
            var product = Mapper.Map<ProductViewModel, Product>(productViewModel);

            // save product
            await _appDbContext.Products.AddAsync(product);
            await _appDbContext.SaveChangesAsync();

            // save size
            if (productViewModel.Sizes != null)
            {
                foreach (var size in productViewModel.Sizes)
                {
                    await _appDbContext.Sizes.AddAsync(new Size { Name = size, Code = size, ProductId = product.Id });
                }
            }

            await _appDbContext.SaveChangesAsync();
            return product;
        }

        /// <summary>
        /// Update product.
        /// </summary>
        /// <param name="productViewModel">product view model.</param>
        /// <returns></returns>
        public async Task<Product> UpdateProductAsync(ProductViewModel productViewModel)
        {
            var productItem = await _appDbContext.Products.SingleOrDefaultAsync(i => i.Id == productViewModel.Id);

            if (productItem == null)
                return null;

            productItem = Mapper.Map<ProductViewModel, Product>(productViewModel);
            _appDbContext.Products.Update(productItem);

            // save size
            if (productViewModel.Sizes != null)
            {
                // deleted contains Size Item
                var deletedSizes = _appDbContext.Sizes.Where(i => i.ProductId == productViewModel.Id);
                _appDbContext.Sizes.RemoveRange(deletedSizes);

                // add new Size
                foreach (var size in productViewModel.Sizes)
                {
                    await _appDbContext.Sizes.AddAsync(new Size { Name = size, Code = size, ProductId = productItem.Id });
                }
            }

            await _appDbContext.SaveChangesAsync();
            return productItem;
        }

        /// <summary>
        /// Delete product.
        /// </summary>
        /// <param name="id">product id.</param>
        /// <returns></returns>
        public async Task<bool> DeleteProductAsync(int id)
        {
            var product = await _appDbContext.Products.SingleOrDefaultAsync(i => i.Id == id);

            if (product == null)
            {
                return false;
            }

            _appDbContext.Products.Remove(product);
            await _appDbContext.SaveChangesAsync();

            return true;
        }

        /// <summary>
        /// Get list product brands.
        /// </summary>
        /// <returns></returns>
        public async Task<ActionResult<List<ProductBrand>>> ProductBrandsAsync()
        {
            return await _appDbContext.ProductBrands.AsNoTracking().ToListAsync();
        }
    }
}
