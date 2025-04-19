namespace LSTY.Sdtd.ServerAdmin.Data.Abstractions
{
    public class EntityBase : Entity, ICreatedOn, IModifiedOn
    {
        public required DateTime CreatedOn { get; set; }
        public DateTime ModifiedOn { get; set; }
    }
}
