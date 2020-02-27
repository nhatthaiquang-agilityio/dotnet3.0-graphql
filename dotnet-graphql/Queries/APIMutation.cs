using dotnet_graphql.GraphQL.InputTypes;
using dotnet_graphql.GraphQL.ObjectTypes;
using dotnet_graphql.Models;
using dotnet_graphql.Services;
using GraphQL.Types;
using ProductType = dotnet_graphql.GraphQL.ObjectTypes.ProductType;

namespace dotnet_graphql.Queries
{
    public class APIMutation : ObjectGraphType
    {
        public APIMutation(ProductService productService, IUserService userService)
        {
            Name = "Mutation";

            Field<ProductType>(
                "createProduct",
                arguments: new QueryArguments(
                    new QueryArgument<NonNullGraphType<ProductInputType>> { Name = "product" }
            ),
            resolve: context =>
            {
                var product = context.GetArgument<ProductViewModel>("product");
                return productService.Create(product);
            });

            Field<ProductType>(
                "updateProduct",
                arguments: new QueryArguments(
                    new QueryArgument<NonNullGraphType<ProductInputType>> { Name = "product" }
            ),
            resolve: context =>
            {
                var product = context.GetArgument<ProductViewModel>("product");
                return productService.UpdateProductAsync(product);
            });

            Field<UserType>(
                "createUser",
                arguments: new QueryArguments(
                    new QueryArgument<NonNullGraphType<UserInputType>> { Name = "user" }
                ),
                resolve: context =>
                {
                    var user = context.GetArgument<UserViewModel>("user");
                    return userService.CreateUser(user);
                });
        }
    }
}
