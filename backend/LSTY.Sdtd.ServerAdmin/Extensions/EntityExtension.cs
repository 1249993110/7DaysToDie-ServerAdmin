

namespace LSTY.Sdtd.ServerAdmin.Extensions
{
    internal static class EntityExtension
    {
        public static EntityBasicInfoDto ToEntityBasicInfo(this EntityAlive entityAlive)
        {
            return new EntityBasicInfoDto()
            {
                EntityId = entityAlive.entityId,
                EntityName = entityAlive.EntityName,
                Position = entityAlive.position.ToPosition(),
                EntityType = entityAlive is EntityPlayer && entityAlive.IsClientControlled() ? Shared.Constants.EntityType.OnlinePlayer : (Shared.Constants.EntityType)entityAlive.entityType
            };
        }
    }
}
