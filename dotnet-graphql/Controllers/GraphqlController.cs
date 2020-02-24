using dotnet_graphql.Queries;
using GraphQL;
using GraphQL.Types;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace dotnet_graphql.Controllers
{
    [Route(Startup.GraphQlPath)]
    public class GraphqlController : ControllerBase
    {
        private readonly ISchema _schema;

        public GraphqlController(ISchema schema)
        {
            _schema = schema;
        }

        /// <summary>
        /// Post query graphQL
        /// </summary>
        /// <param name="query">query graphql.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] GraphQLQuery query)
        {
            // query null
            if (query == null) { throw new ArgumentNullException(nameof(query)); }

            var inputs = query.Variables.ToInputs();
            var executionOptions = new ExecutionOptions
            {
                Schema = _schema,
                Query = query.Query,
                Inputs = inputs
            };

            var result = await new DocumentExecuter().ExecuteAsync(executionOptions)
                .ConfigureAwait(false);

            // has error
            if (result.Errors?.Count > 0)
                return BadRequest(result);

            return Ok(result);
        }
    }
}
