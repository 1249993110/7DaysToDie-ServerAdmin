namespace LSTY.Sdtd.ServerAdmin.Shared.Models
{
    /// <summary>
    /// 
    /// </summary>
    public enum RespawnType
    {
        /// <summary>
        /// 
        /// </summary>
        NewGame = 0,

        /// <summary>
        /// 
        /// </summary>
        LoadedGame = 1,

        /// <summary>
        /// 
        /// </summary>
        Died = 2,

        /// <summary>
        /// 
        /// </summary>
        Teleport = 3,

        /// <summary>
        /// New player join
        /// </summary>
        EnterMultiplayer = 4,

        /// <summary>
        /// Old player join
        /// </summary>
        JoinMultiplayer = 5,

        /// <summary>
        /// 
        /// </summary>
        Unknown = 6
    }
}