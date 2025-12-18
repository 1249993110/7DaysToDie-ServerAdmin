namespace LSTY.Sdtd.ServerAdmin.Shared.EventArgs
{
    public class EntitySpawnedEventArgs : System.EventArgs
    {
        /// <summary>
        /// 
        /// </summary>
        public required EntityBasicInfoDto SpawnedEntity { get; set; }

        /// <summary>
        ///
        /// </summary>
        public required DateTime Timestamp { get; set; }
    }
}
