using HarmonyLib;
using LSTY.Sdtd.ServerAdmin.Extensions;
using LSTY.Sdtd.ServerAdmin.Shared.Models;

namespace LSTY.Sdtd.ServerAdmin.HarmonyPatchers
{
    [HarmonyPatch(typeof(World))]
    internal static class WorldPatcher
    {
        private static Action<EntityBasicInfo>? _entitySpawnedCallback;

        public static void Init(Action<EntityBasicInfo> entitySpawnedCallback)
        {
            _entitySpawnedCallback = entitySpawnedCallback;
        }

        [HarmonyPostfix]
        [HarmonyPatch(nameof(World.SpawnEntityInWorld))]
        public static void After_SpawnEntityInWorld(Entity _entity)
        {
            if (_entity is EntityAlive entityAlive)
            {
                _entitySpawnedCallback?.Invoke(entityAlive.ToEntityBasicInfo());
            }
        }
    }
}