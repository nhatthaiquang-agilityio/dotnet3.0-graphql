using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using dotnet_graphql.Models;

namespace dotnet_graphql.Data
{
    public class AppContextSeed
    {
        public async Task SeedAsync(AppDbContext context)
        {
            if (!context.ProductTypes.Any())
                await context.ProductTypes.AddRangeAsync(GetPreconfiguredProductTypes());

            if (!context.ProductBrands.Any())
                await context.ProductBrands.AddRangeAsync(GetPreconfiguredProductBrands());

            await context.SaveChangesAsync();

            if (!context.Products.Any())
            {
                await context.Products.AddRangeAsync(GetPreconfiguredProducts());
                await context.SaveChangesAsync();

                foreach (var product in context.Products.ToList())
                {
                    await context.Sizes.AddRangeAsync(new List<Size>
                    {
                        new Size { Name = "S", Code = "S", Product = product },
                        new Size { Name = "L", Code = "L", Product = product },
                        new Size { Name = "M", Code = "M", Product = product },
                        new Size { Name = "XL", Code = "XL", Product = product }
                    });
                }

                await context.SaveChangesAsync();
            }

            if (!context.Books.Any())
            {
                await InitBooks(context);
            }
        }

        private static IEnumerable<ProductBrand> GetPreconfiguredProductBrands()
        {
            return new List<ProductBrand>
            {
                new ProductBrand { Brand = "Nike"},
                new ProductBrand { Brand = "Adidas" },
                new ProductBrand { Brand = "Puma" },
                new ProductBrand { Brand = "Uniqlo" },
                new ProductBrand { Brand = "Other" }
            };
        }

        private static IEnumerable<ProductType> GetPreconfiguredProductTypes()
        {
            return new List<ProductType>
            {
                new ProductType { Type = "Shoes" },
                new ProductType { Type = "T-Shirt" },
                new ProductType { Type = "Paint M" }
            };
        }

        private static IEnumerable<Product> GetPreconfiguredProducts()
        {
            return new List<Product>
            {
                new Product {
                    ProductTypeId = 2,
                    ProductBrandId = 1,
                    AvailableStock = 100,
                    Description = "Black Hoodie",
                    Name = "Bot Black Hoodie",
                    Price = 19.5M
                },
                new Product {
                    ProductTypeId = 1,
                    ProductBrandId = 2,
                    AvailableStock = 100,
                    Description = "Black & White Shoes",
                    Name = "Black & White Shoes",
                    Price= 8.50M
                },
                new Product {
                    ProductTypeId = 2,
                    ProductBrandId = 5,
                    AvailableStock = 100,
                    Description = "Prism White T-Shirt",
                    Name = "Prism White T-Shirt",
                    Price = 12
                },
                new Product {
                    ProductTypeId = 2,
                    ProductBrandId = 3,
                    AvailableStock = 100,
                    Description = "Foundation T-shirt",
                    Name = "Foundation T-shirt",
                    Price = 12
                },
                new Product {
                    ProductTypeId = 3,
                    ProductBrandId = 4,
                    AvailableStock = 100,
                    Description = "Roslyn Red trousers pants",
                    Name = "Roslyn Red trousers pants",
                    Price = 8.5M
                },
                new Product {
                    ProductTypeId = 2,
                    ProductBrandId = 2,
                    AvailableStock = 100,
                    Description = "Blue Hoodie",
                    Name = " Blue Hoodie",
                    Price = 12
                },
                new Product {
                    ProductTypeId = 2,
                    ProductBrandId = 5,
                    AvailableStock = 100,
                    Description = "Roslyn Red T-Shirt",
                    Name = "Roslyn Red T-Shirt",
                    Price = 12
                },
                new Product {
                    ProductTypeId = 2,
                    ProductBrandId = 1,
                    AvailableStock = 100,
                    Description = "Kudu Purple Hoodie",
                    Name = "Kudu Purple Hoodie",
                    Price = 8.5M
                },
                new Product {
                    ProductTypeId = 1,
                    ProductBrandId = 5,
                    AvailableStock = 100,
                    Description = "White Sneaker",
                    Name = "White Sneaker 11",
                    Price = 12
                },
                new Product {
                    ProductTypeId = 3,
                    ProductBrandId = 3,
                    AvailableStock = 100,
                    Description = "F1 Trouser",
                    Name = "F1 trousers pants",
                    Price = 11
                },
                new Product {
                    ProductTypeId = 3,
                    ProductBrandId = 2,
                    AvailableStock = 100,
                    Description = "Trouser",
                    Name = " Trouser 11",
                    Price = 8.50M

                },
                new Product {
                    ProductTypeId = 2,
                    ProductBrandId = 4,
                    AvailableStock = 100,
                    Description = "Prism White TShirt",
                    Name = "Prism White TShirt",
                    Price = 16
                }
            };
        }

        private static async Task InitBooks(AppDbContext context)
        {
            await context.Authors.AddRangeAsync(new List<Author> {
                new Author { FirstName = "Nick", LastName = "Shaw" },
                new Author { FirstName = "David", LastName = "Coup" },
                new Author { FirstName = "Tom", LastName = "Hank" }
            });
            var category = new Category
            {
                CategoryName = "Network"
            };

            var category1 = new Category
            {
                CategoryName = "Programming"
            };
            await context.Categories.AddRangeAsync(new List<Category> { category, category1 });
            await context.SaveChangesAsync();

            // add book and bookcategory
            var book = new Book
            {
                BookName = "Quantum Networking",
                Price = 220,
                AuthorId = 1
            };

            book.BookCategories = new List<BookCategory>
            {
                new BookCategory
                {
                    Category = category,
                    Book = book

                }
            };

            // add book and bookcategory
            var book1 = new Book
            {
                BookName = "Advance C#",
                Price = 110,
                AuthorId = 2
            };

            book1.BookCategories = new List<BookCategory>
            {
                new BookCategory
                {
                    Category = category1,
                    Book = book1
                }
            };

            //Now add this book, with all its relationships, to the database
            await context.Books.AddRangeAsync(new List<Book> { book, book1});
            await context.SaveChangesAsync();
        }
    }
}
