using GraphQL.Types;
using dotnet_graphql.Models;

namespace dotnet_graphql.GraphQL
{
    public class BookCategoriesType : ObjectGraphType<BookCategory>
    {
        public BookCategoriesType()
        {
            Field(x => x.BookId).Description("Book Id");
            Field(x => x.CategoryId).Description("Category Id");
            Field(x => x.Category, type: typeof(CategoryType)).Description("Category");
        }
    }
}