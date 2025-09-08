using LSTY.Sdtd.ServerAdmin.Shared.Models;

namespace LSTY.Sdtd.ServerAdmin.Extensions
{
    internal static class EntityExtension
    {
        public static EntityBasicInfo ToEntityBasicInfo(this EntityAlive entityAlive)
        {
            return new EntityBasicInfo()
            {
                EntityId = entityAlive.entityId,
                EntityName = entityAlive.EntityName,
                Position = entityAlive.position.ToPosition(),
                EntityType = entityAlive is EntityPlayer && entityAlive.IsClientControlled() ? Shared.Models.EntityType.OnlinePlayer : (Shared.Models.EntityType)entityAlive.entityType
            };
        }
    }
}
