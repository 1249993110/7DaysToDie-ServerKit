using HarmonyLib;

namespace SdtdServerKit.HarmonyPatchers
{
    [HarmonyPatch(typeof(World))]
    internal class WorldPatcher
    {
        [HarmonyPostfix]
        [HarmonyPatch(nameof(World.SpawnEntityInWorld))]
        public static void After_SpawnEntityInWorld(Entity _entity)
        {
            if (_entity is EntityAlive entityAlive)
            {
                ModEventHub.OnEntitySpawned(new EntityInfo()
                {
                    EntityId = entityAlive.entityId,
                    EntityName = entityAlive.EntityName,
                    Position = entityAlive.position.ToPosition(),
                    EntityType = (SdtdServerKit.Models.EntityType)entityAlive.entityType
                });
            }
        }
    }
}