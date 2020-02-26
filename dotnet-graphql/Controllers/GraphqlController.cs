using dotnet_graphql.Queries;
using GraphQL;
using GraphQL.Types;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GraphQL.Validation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;

namespace dotnet_graphql.Controllers
{
    [Route(Startup.GraphQlPath)]
    public class GraphqlController : ControllerBase
    {
        private readonly ISchema _schema;
        private readonly IValidationRule _validationRule;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public GraphqlController(ISchema schema,
            IValidationRule validationRule,
            IHttpContextAccessor httpContextAccessor)
        {
            _schema = schema;
            _validationRule = validationRule;
            _httpContextAccessor = httpContextAccessor;
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

            Console.WriteLine("http context");
            Console.WriteLine(_httpContextAccessor.HttpContext.User.Identity.IsAuthenticated);

            var inputs = query.Variables.ToInputs();
            var executionOptions = new ExecutionOptions
            {
                Schema = _schema,
                Query = query.Query,
                Inputs = inputs,
                ValidationRules = new List<IValidationRule>{ _validationRule },
                UserContext = _httpContextAccessor.HttpContext.User
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
