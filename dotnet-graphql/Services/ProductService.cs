using dotnet_graphql.Models;
using dotnet_graphql.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;


namespace dotnet_graphql.Services
{
    public class ProductService
    {
        private readonly AppDbContext _appDBContext;

        public ProductService(AppDbContext context)
        {
            _appDBContext = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<IEnumerable<Product>> GetProducts()
        {
            return await _appDBContext.Products.Include(p => p.Sizes)
                .Include(p => p.ProductType).Include(p => p.ProductBrand)
                .AsNoTracking().ToListAsync();
        }

        public async Task<Product> GetProduct(int id)
        {
            return await _appDBContext.Products.Include(p => p.Sizes)
                .Include(a => a.ProductType).Include(a => a.ProductBrand).AsNoTracking()
                .SingleOrDefaultAsync(p => p.Id == id);
        }

        public async Task<IEnumerable<Size>> GetSizeOfProduct(int id)
        {
            return await _appDBContext.Sizes.Where(p => p.ProductId == id).ToListAsync();
        }

        public async Task<Product> Create(ProductViewModel productViewModel)
        {
            Product product = new Product
            {
                ProductBrandId = productViewModel.ProductBrandId,
                ProductTypeId = productViewModel.ProductTypeId,
                Name = productViewModel.Name,
                Price = productViewModel.Price,
                Description = productViewModel.Description,
                AvailableStock = productViewModel.AvailableStock
            };

            // save product
            await _appDBContext.Products.AddAsync(product);

            // save size
            if (productViewModel.Sizes != null)
            {
                foreach (string size in productViewModel.Sizes)
                {
                    await _appDBContext.Sizes.AddAsync(new Size { Name = size, Code = size, ProductId = product.Id });
                }
            }

            await _appDBContext.SaveChangesAsync();
            return product;
        }

        public async Task<Product> UpdateProductAsync(ProductViewModel productViewModel)
        {
            Product productItem = await _appDBContext.Products.SingleOrDefaultAsync(i => i.Id == productViewModel.Id);

            if (productItem == null)
                return null;

            // set values
            productItem.ProductBrandId = productViewModel.ProductBrandId;
            productItem.ProductTypeId = productViewModel.ProductTypeId;
            productItem.Name = productViewModel.Name;
            productItem.Price = productViewModel.Price;
            productItem.Description = productViewModel.Description;
            productItem.AvailableStock = productViewModel.AvailableStock;

            _appDBContext.Products.Update(productItem);

            // save size
            if (productViewModel.Sizes != null)
            {
                // deleted contains Size Item
                var deletedSizes = _appDBContext.Sizes.Where(i => i.ProductId == productViewModel.Id);
                _appDBContext.Sizes.RemoveRange(deletedSizes);

                // add new Size
                foreach (string size in productViewModel.Sizes)
                {
                    await _appDBContext.Sizes.AddAsync(new Size { Name = size, Code = size, ProductId = productItem.Id });
                }
            }

            await _appDBContext.SaveChangesAsync();
            return productItem;
        }

        public async Task<bool> DeleteProductAsync(int id)
        {
            var product = await _appDBContext.Products.SingleOrDefaultAsync(i => i.Id == id);

            if (product == null)
            {
                return false;
            }

            _appDBContext.Products.Remove(product);
            await _appDBContext.SaveChangesAsync();

            return true;
        }

        public async Task<ActionResult<List<ProductBrand>>> ProductBrandsAsync()
        {
            return await _appDBContext.ProductBrands.AsNoTracking().ToListAsync();
        }
    }
}