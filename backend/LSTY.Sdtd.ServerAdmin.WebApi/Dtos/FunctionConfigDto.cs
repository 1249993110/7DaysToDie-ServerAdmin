namespace LSTY.Sdtd.ServerAdmin.WebApi.Dtos
{
    /// <summary>
    /// 
    /// </summary>
    public class FunctionConfigDto
    {
        /// <summary>
        /// Game server ID.
        /// </summary>
        public required string GameServerId { get; set; }

        /// <summary>
        /// Function name.
        /// </summary>
        public required string FunctionName { get; set; }

        /// <summary>
        /// Function settings.
        /// </summary>
        public required Dictionary<string, object?>? Settings { get; set; }
    }
}
