using GraphQL.Types;
using dotnet_graphql.Models;

namespace dotnet_graphql.GraphQL.ObjectTypes
{
    public class AuthorType : ObjectGraphType<Author>
    {
        public AuthorType()
        {
            Field(x => x.Id).Description("Id of an author");
            Field(x => x.FirstName).Description("FirstName of an author");
            Field(x => x.LastName).Description("LastName of an author");
            Field(x => x.Address).Description("Address");
        }
    }
}
