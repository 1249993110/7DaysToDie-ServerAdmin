namespace LSTY.Sdtd.ServerAdmin.WebApi.Dtos
{
    /// <summary>
    /// Represents the data required to create a new function config.
    /// </summary>
    public class FunctionConfigCreateDto : FunctionConfigUpdateDto
    {
        /// <summary>
        /// The name of the function config.
        /// </summary>
        public required string FunctionName { get; set; }
    }
}
