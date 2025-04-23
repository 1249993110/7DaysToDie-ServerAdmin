using LSTY.Sdtd.ServerAdmin.WebApi.Authorization;
using Microsoft.AspNetCore.Authorization;
using NSwag.Generation.Processors.Contexts;
using NSwag.Generation.Processors;
using NSwag;

namespace LSTY.Sdtd.ServerAdmin.WebApi.OperationProcessors
{
    /// <summary>
    /// Adds the game server ID header parameter to the operation if the controller is authorized with the GameServerOwner policy.
    /// </summary>
    public class AddGameServerIdHeaderParameter : IOperationProcessor
    {
        private const string paramName = "gameServerId";

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public bool Process(OperationProcessorContext context)
        {
            if (context.ControllerType.CustomAttributes.Any(p => p.AttributeType == typeof(AuthorizeAttribute)
                    && p.ConstructorArguments.FirstOrDefault().Value?.ToString() == AuthorizationPolicys.GameServerOwner))
            {
                if(context.Parameters.Any(p => p.Key.Name == paramName) == false)
                {
                    context.OperationDescription.Operation.Parameters.Add(new OpenApiParameter
                    {
                        Name = paramName,
                        Kind = OpenApiParameterKind.Header,
                        Type = NJsonSchema.JsonObjectType.String,
                        IsRequired = true,
                        Description = "The ID of the game server."
                    });
                }
            }

            return true;
        }
    }
}
