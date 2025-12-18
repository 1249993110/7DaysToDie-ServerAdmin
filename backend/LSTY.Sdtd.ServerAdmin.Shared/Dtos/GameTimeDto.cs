namespace LSTY.Sdtd.ServerAdmin.Shared.Dtos
{
    /// <summary>
    /// Game time
    /// </summary>
    public class GameTimeDto
    {
        /// <summary>
        /// Number of days
        /// </summary>
        public required int Days { get; set; }

        /// <summary>
        /// Number of hours
        /// </summary>
        public required int Hours { get; set; }

        /// <summary>
        /// Number of minutes
        /// </summary>
        public required int Minutes { get; set; }
    }
}
