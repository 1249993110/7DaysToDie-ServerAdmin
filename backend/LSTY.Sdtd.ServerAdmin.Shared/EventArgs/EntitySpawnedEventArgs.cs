using LSTY.Sdtd.ServerAdmin.Shared.Models;

namespace LSTY.Sdtd.ServerAdmin.Shared.EventArgs
{
    public class EntitySpawnedEventArgs : System.EventArgs
    {
        /// <summary>
        /// 
        /// </summary>
        public required EntityBasicInfo SpawnedEntity { get; set; }

        /// <summary>
        ///
        /// </summary>
        public required DateTime Timestamp { get; set; }
    }
}
