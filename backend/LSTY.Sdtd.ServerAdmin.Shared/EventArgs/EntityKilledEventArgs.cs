using LSTY.Sdtd.ServerAdmin.Shared.Models;

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
        public required EntityBasicInfo Victim { get; set; }

        /// <summary>
        /// The killer entity
        /// </summary>
        public EntityBasicInfo? Killer { get; set; }

        /// <summary>
        ///
        /// </summary>
        public required DateTime Timestamp { get; set; }
    }
}