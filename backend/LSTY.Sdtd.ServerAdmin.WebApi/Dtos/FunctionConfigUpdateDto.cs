using System.Text.Json.Nodes;

namespace LSTY.Sdtd.ServerAdmin.WebApi.Dtos
{
    /// <summary>
    /// Represents the data required to update existing function settings.
    /// </summary>
    public class FunctionConfigUpdateDto
    {
        /// <summary>
        /// A dictionary of key-value pairs representing the function's configuration settings.
        /// </summary>
        public required Dictionary<string, object?> Settings { get; set; }
    }
}
