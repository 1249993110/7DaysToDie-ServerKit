using HarmonyLib;
using SdtdServerKit.Hooks;
using System.Reflection;

namespace SdtdServerKit.HarmonyPatchers
{
    [HarmonyPatch(typeof(World))]
    internal class WorldPatcher
    {
        [HarmonyPostfix]
        [HarmonyPatch(nameof(World.SpawnEntityInWorld))]
        public static void OnAfterSpawnEntityInWorld(Entity _entity)
        {
            if (_entity is EntityAlive entityAlive)
            {
                ModEventHook.OnEntitySpawned(new EntityInfo()
                {
                    EntityId = entityAlive.entityId,
                    EntityName = entityAlive.EntityName,
                    Position = entityAlive.position.ToPosition(),
                    EntityType = (SdtdServerKit.Shared.Models.EntityType)entityAlive.entityType
                });
            }
        }
    }
}