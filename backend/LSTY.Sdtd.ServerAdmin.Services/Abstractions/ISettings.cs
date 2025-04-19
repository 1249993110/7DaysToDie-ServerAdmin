namespace LSTY.Sdtd.ServerAdmin.Services.Abstractions
{
    /// <summary>
    /// Interface for settings.
    /// </summary>
    public interface ISettings
    {
        /// <summary>
        /// Indicates whether the function is enabled.
        /// </summary>
        public bool IsEnabled { get; set; }
    }
}