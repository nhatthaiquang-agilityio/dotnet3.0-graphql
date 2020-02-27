using GraphQL.Types;
using dotnet_graphql.Models;

namespace dotnet_graphql.GraphQL.ObjectTypes
{
    public class BookType : ObjectGraphType<Book>
    {
        public BookType()
        {
            Field(x => x.Id).Description("Id of a book");
            Field(x => x.BookName).Description("Title");
            Field(x => x.Price).Description("Price of Book");
            Field(x => x.AuthorId).Description("Author Id");
            Field(x => x.Author, type: typeof(AuthorType)).Description("Author");
            Field(x => x.BookCategories, type: typeof(ListGraphType<BookCategoriesType>))
                .Description("Categories of book");
        }
    }
}
