namespace LSTY.Sdtd.ServerAdmin.Data.Options
{
    /// <summary>
    /// Contains settings for RavenDB, such as the URL to the database.
    /// </summary>
    public class RavenOptions
    {
        /// <summary>
        /// The URLs where the database resides.
        /// </summary>
        public required string[] Urls { get; set; }

        /// <summary>
        /// The name of the database.
        /// </summary>
        public required string DatabaseName { get; set; }
    }
}
