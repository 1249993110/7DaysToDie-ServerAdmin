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
        internal const string Name = "gameServerId";

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
                var parameters = context.OperationDescription.Operation.Parameters;

                const string description = "The ID of the game server.";

                var param = parameters.FirstOrDefault(p => Name.Equals(p.Name, StringComparison.OrdinalIgnoreCase));
                if (param == null)
                {
                    parameters.Add(new OpenApiParameter
                    {
                        Name = Name,
                        Kind = OpenApiParameterKind.Header,
                        Type = NJsonSchema.JsonObjectType.String,
                        IsRequired = true,
                        Description = description
                    });
                }
                else
                {
                    param.IsRequired = true;
                    param.Description = description;
                }
            }

            return true;
        }
    }
}
