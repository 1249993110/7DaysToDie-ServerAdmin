﻿using HarmonyLib;
using LSTY.Sdtd.ServerAdmin.Overseer.Extensions;
using LSTY.Sdtd.ServerAdmin.Shared.Models;

namespace LSTY.Sdtd.ServerAdmin.Overseer.HarmonyPatchers
{
    [HarmonyPatch(typeof(World))]
    internal static class WorldPatcher
    {
        private static Action<EntityInfo>? _entitySpawnedCallback;

        public static void Init(Action<EntityInfo> entitySpawnedCallback)
        {
            _entitySpawnedCallback = entitySpawnedCallback;
        }

        [HarmonyPostfix]
        [HarmonyPatch(nameof(World.SpawnEntityInWorld))]
        public static void After_SpawnEntityInWorld(Entity _entity)
        {
            if (_entity is EntityAlive entityAlive)
            {
                _entitySpawnedCallback?.Invoke(entityAlive.ToEntityInfo());
            }
        }
    }
}