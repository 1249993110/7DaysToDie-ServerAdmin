namespace LSTY.Sdtd.ServerAdmin.Shared.Dtos
{
    /// <summary>
    /// Map Info
    /// </summary>
    public class MapInfoDto
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
