namespace LSTY.Sdtd.ServerAdmin.WebApi.Dtos
{
    /// <summary>
    /// Represents the data required to create a new function setting.
    /// </summary>
    public class CreateFunctionSettingsDto : UpdateFunctionSettingsDto
    {
        /// <summary>
        /// The name of the function being configured.
        /// </summary>
        public string? FunctionName { get; set; }
    }
}
