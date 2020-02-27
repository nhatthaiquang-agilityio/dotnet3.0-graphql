using GraphQL.Types;
using dotnet_graphql.Models;

namespace dotnet_graphql.GraphQL.InputTypes
{
    public class UserInputType : InputObjectGraphType<UserViewModel>
    {
        public UserInputType()
        {
            Name = "UserInput";
            Field<StringGraphType>("FirstName");
            Field<StringGraphType>("LastName");
            Field<StringGraphType>("Username");
            Field<StringGraphType>("Password");
        }
    }
}
