namespace LSTY.Sdtd.ServerAdmin.Shared.EventArgs
{
    /// <summary>
    /// 
    /// </summary>
    public class EntityKilledEventArgs : System.EventArgs
    {
        /// <summary>
        /// The dead entity
        /// </summary>
        public required EntityBasicInfoDto Victim { get; set; }

        /// <summary>
        /// The killer entity
        /// </summary>
        public EntityBasicInfoDto? Killer { get; set; }

        /// <summary>
        ///
        /// </summary>
        public required DateTime Timestamp { get; set; }
    }
}