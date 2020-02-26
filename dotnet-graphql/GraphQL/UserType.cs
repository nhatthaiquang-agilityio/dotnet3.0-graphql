using GraphQL.Types;
using dotnet_graphql.Models;

namespace dotnet_graphql.GraphQL
{
    public class UserType : ObjectGraphType<User>
    {
        public UserType()
        {
            Name = nameof(User);
            Field(x => x.FirstName).Description("FirstName");
            Field(x => x.LastName).Description("LastName");
            Field(x => x.Username).Description("Username");
            Field(x => x.Password).Description("Password");
        }
    }
}
