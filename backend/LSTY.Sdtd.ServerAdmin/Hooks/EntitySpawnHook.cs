using HarmonyLib;
using LSTY.Sdtd.ServerAdmin.Extensions;


namespace LSTY.Sdtd.ServerAdmin.Hooks
{
    [HarmonyPatch(typeof(World), nameof(World.SpawnEntityInWorld))]
    internal static class EntitySpawnHook
    {
        internal static event Action<EntityBasicInfoDto>? OnEntitySpawned;

        [HarmonyPostfix]
        public static void Postfix(Entity _entity)
        {
            if (_entity is EntityAlive entityAlive)
            {
                OnEntitySpawned?.Invoke(entityAlive.ToEntityBasicInfo());
            }
        }
    }
}