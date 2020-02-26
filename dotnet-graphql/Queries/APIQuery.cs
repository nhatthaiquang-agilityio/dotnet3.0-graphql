using dotnet_graphql.Services;
using dotnet_graphql.GraphQL;
using GraphQL;
using GraphQL.Types;

namespace dotnet_graphql.Queries
{
    public class APIQuery : ObjectGraphType
    {
        public APIQuery(BookService bookService, ProductService productService, AuthorService authorService)
        {
            Field<BookType>(
                "Book",
                arguments: new QueryArguments(new QueryArgument<IntGraphType> { Name = "id" }),
                resolve: context =>
                {
                    var id = context.GetArgument<int>("id");
                    return bookService.GetBook(id);
                }
            );

            Field<ListGraphType<BookType>>(
                "Books",
                resolve: context => bookService.GetBooks()
            );


            Field<ListGraphType<AuthorType>>(
                "Authors",
                resolve: context => authorService.GetAuthors()
            );

            Field<AuthorType>(
                "Author",
                arguments: new QueryArguments(new QueryArgument<IntGraphType> { Name = "id" }),
                resolve: context =>
                {
                    var id = context.GetArgument<int>("id");
                    return authorService.GetAuthor(id);
                }
            );

            Field<ProductType>(
                "Product",
                arguments: new QueryArguments(new QueryArgument<IntGraphType> { Name = "id" }),
                resolve: context =>
                {
                    var id = context.GetArgument<int>("id");
                    return productService.GetProduct(id);
                }
            );

            Field<ListGraphType<ProductType>>(
                "Products",
                resolve: context => productService.GetProducts()
            );

            Field<BookCategoriesType>(
                "BookCategories",
                arguments: new QueryArguments(new QueryArgument<IntGraphType> { Name = "id" }),
                resolve: context =>
                {
                    var id = context.GetArgument<int>("id");
                    return bookService.GetBookCategory(id);
                }
            );

            Field<CategoryType>(
                "Category",
                arguments: new QueryArguments(new QueryArgument<IntGraphType> { Name = "id" }),
                resolve: context =>
                {
                    var id = context.GetArgument<int>("id");
                    return bookService.GetCategory(id);
                }
            );

            Field<ListGraphType<SizeType>>(
                "Sizes",
                arguments: new QueryArguments(new QueryArgument<IntGraphType> { Name = "id" }),
                resolve: context =>
                {
                    var id = context.GetArgument<int>("id");
                    return productService.GetSizeOfProduct(id);
                }
            );
        }
    }
}
