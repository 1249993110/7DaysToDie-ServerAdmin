namespace LSTY.Sdtd.ServerAdmin.Data.Abstractions
{
    public class FileEntityBase : FileEntity, ICreatedOn, IModifiedOn
    {
        public DateTime CreatedOn { get; set; }
        public DateTime ModifiedOn { get; set; }
    }
}
