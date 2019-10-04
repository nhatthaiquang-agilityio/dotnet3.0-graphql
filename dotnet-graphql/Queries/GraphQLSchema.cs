using System;
using GraphQL.Types;

namespace dotnet_graphql.Queries
{
    public class GraphQLSchema : Schema
    {
        //public GraphQLSchema(IDependencyResolver resolver) : base(resolver)
        //{
        //    Query = resolver.Resolve<APIQuery>();
        //    Mutation = resolver.Resolve<ProductMutation>();
        //}

        public GraphQLSchema(Func<Type, GraphType> resolve) : base(resolve)
        {
            Query = (APIQuery)resolve(typeof(APIQuery));
            Mutation = (ProductMutation)resolve(typeof(ProductMutation));
        }
    }
}
