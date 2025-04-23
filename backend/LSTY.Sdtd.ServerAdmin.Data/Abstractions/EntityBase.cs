namespace LSTY.Sdtd.ServerAdmin.Data.Abstractions
{
    public class EntityBase : Entity, ICreatedOn, IModifiedOn
    {
        /// <summary>
        /// The date and time when the entity was created. If manually set, it should be in UTC format and must set ID of the entity.
        /// </summary>
        public DateTime CreatedOn { get; set; }
        public DateTime ModifiedOn { get; set; }
    }
}
