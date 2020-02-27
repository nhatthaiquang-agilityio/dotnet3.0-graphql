using GraphQL.Types;
using dotnet_graphql.Models;

namespace dotnet_graphql.GraphQL.ObjectTypes
{
    public class CategoryType : ObjectGraphType<Category>
    {
        public CategoryType()
        {
            Field(x => x.CategoryId).Description("Id");
            Field(x => x.CategoryName).Description("Name");
        }
    }
}
