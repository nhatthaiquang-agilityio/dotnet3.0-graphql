using GraphQL.Types;
using dotnet_graphql.Models;

namespace dotnet_graphql.GraphQL
{
    public class SizeType : ObjectGraphType<Size>
    {
        public SizeType()
        {
            Field(x => x.Id).Description("Id");
            Field(x => x.Name).Description("Name ");
            Field(x => x.Code).Description("Code");
            Field(x => x.ProductId).Description("Product Id");
        }
    }
}
