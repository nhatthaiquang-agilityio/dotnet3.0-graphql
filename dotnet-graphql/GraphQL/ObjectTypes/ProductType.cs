using GraphQL.Types;
using dotnet_graphql.Models;

namespace dotnet_graphql.GraphQL.ObjectTypes
{
    public class ProductType : ObjectGraphType<Product>
    {
        public ProductType()
        {
            Field(x => x.Id).Description("Id of a product");
            Field(x => x.Name).Description("Name of Product");
            Field(x => x.Price).Description("Price of Book");
            Field(x => x.Description).Description("Description of Book");
            Field(x => x.AvailableStock).Description("AvailableStock of Book");
            Field(x => x.Sizes, type: typeof(ListGraphType<SizeType>)).Description("Sizes");
            Field(x => x.ProductTypeId).Description("ProductTypeId of Book");
            Field(x => x.ProductBrandId).Description("Product Brand Id");
        }
    }
}
