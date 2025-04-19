namespace LSTY.Sdtd.ServerAdmin.Shared.Constants
{
    /// <summary>
    /// Common constants
    /// </summary>
    public struct Common
    {
        /// <summary>
        /// Represents a non-player entity.
        /// </summary>
        public const string NonPlayer = "-non-player-";

        /// <summary>
        /// The name of the company.
        /// </summary>
        public const string CompanyName = "LSTY";

        /// <summary>
        /// Timeout duration for SSL handshake.
        /// <para>Value is in seconds.</para>
        /// </summary>
        public const int SslHandshakeTimeout = 10;
    }
}