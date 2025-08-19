using NSwag;
using NSwag.Generation.Processors;
using NSwag.Generation.Processors.Contexts;
using System.Reflection;
using System.Web.Http;

namespace LSTY.Sdtd.ServerAdmin.WebApi
{
    internal class OperationSecurityScopeProcessor : IOperationProcessor
    {
        private readonly string[] _names;

        /// <summary>
        /// Initializes a new instance of the NSwag.Generation.Processors.Security.OperationSecurityScopeProcessor class.
        /// </summary>
        /// <param name="names">The security definition names.</param>
        public OperationSecurityScopeProcessor(params string[] names)
        {
            _names = names;
        }

        /// <summary>
        /// Processes the specified method information.
        /// </summary>
        /// <param name="context"></param>
        /// <returns>true if the operation should be added to the Swagger specification.</returns>
        public bool Process(OperationProcessorContext context)
        {
            if (context.OperationDescription.Operation.Security == null)
            {
                context.OperationDescription.Operation.Security = new List<OpenApiSecurityRequirement>();
            }

            var scopes = GetScopes(context.OperationDescription, context.MethodInfo);
            if (scopes.Any())
            {
                var openApiSecurityRequirement = new OpenApiSecurityRequirement();
                foreach (var name in _names)
                {
                    openApiSecurityRequirement.Add(name, scopes);
                }
                context.OperationDescription.Operation.Security.Add(openApiSecurityRequirement);
            }

            return true;
        }

        /// <summary>
        /// Gets the security scopes for an operation.
        /// </summary>
        /// <param name="operationDescription">The operation description.</param>
        /// <param name="methodInfo">The method information.</param>
        /// <returns></returns>
        protected virtual IEnumerable<string> GetScopes(OpenApiOperationDescription operationDescription, MethodInfo methodInfo)
        {
            var source = methodInfo.GetCustomAttributes().Concat(methodInfo.DeclaringType.GetTypeInfo().GetCustomAttributes());

            if (source.Any(i => i is AllowAnonymousAttribute))
            {
                return Enumerable.Empty<string>();
            }

            var source2 = source.Where(a => a is AuthorizeAttribute);
            if (source2.Any() == false)
            {
                return Enumerable.Empty<string>();
            }

            var result = new List<string>();
            foreach (var item in source2)
            {
                if (item is AuthorizeAttribute authorizeAttribute)
                {
                    if (authorizeAttribute.Roles != null)
                    {
                        result.AddRange(authorizeAttribute.Roles.Split(','));
                    }
                }
            }

            return result.Distinct();
        }
    }
}