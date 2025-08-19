namespace LSTY.Sdtd.ServerAdmin.Shared.Models
{
    /// <summary>
    /// 
    /// </summary>
    public class EntityInfo
    {
        /// <summary>
        /// 
        /// </summary>
        public required int EntityId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public required string EntityName { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public required EntityType EntityType { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public required Position? Position { get; set; }
    }
}
