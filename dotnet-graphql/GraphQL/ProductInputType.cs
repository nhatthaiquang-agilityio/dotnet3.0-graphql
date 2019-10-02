using dotnet_graphql.Models;
using GraphQL.Types;

namespace dotnet_graphql.GraphQL
{
    public class ProductInputType : InputObjectGraphType<ProductViewModel>
    {
        public ProductInputType()
        {
            Name = "ProductInput";
            Field<IntGraphType>("id");
            Field<StringGraphType>("name");
            Field<StringGraphType>("description");
            Field<IntGraphType>("availableStock");
            Field<DecimalGraphType>("price");
            Field<ListGraphType<StringGraphType>>("sizes");

            Field<IntGraphType>("productTypeId");
            Field<IntGraphType>("productBrandId");
        }
    }
}
