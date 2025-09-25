namespace LSTY.Sdtd.ServerAdmin.Shared.Models
{
    /// <summary>
    /// Map Info
    /// </summary>
    public class MapInfo
    {
        /// <summary>
        /// Block Size
        /// </summary>
        public required int BlockSize { get; set; }

        /// <summary>
        /// Max Zoom
        /// </summary>
        public required int MaxZoom { get; set; }
    }
}
