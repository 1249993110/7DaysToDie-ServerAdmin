namespace LSTY.Sdtd.ServerAdmin.Data.Options
{
    /// <summary>
    /// Contains settings for RavenDB, such as the URL to the database.
    /// </summary>
    public class DatabaseOptions
    {
        /// <summary>
        /// The connection string to the database.
        /// </summary>
        public required string ConnectionString { get; set; }
        /// <summary>
        /// The name of the database.
        /// </summary>
        public required string DatabaseName { get; set; }
    }
}
