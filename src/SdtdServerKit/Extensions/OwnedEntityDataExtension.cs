namespace SdtdServerKit.Extensions
{
    public static class OwnedEntityDataExtension
    {
        public static OwnedEntity ToModel(this OwnedEntityData entity)
        {
            return new OwnedEntity()
            {
                Id = entity.Id,
                ClassId = entity.ClassId,
                LastKnownPosition = entity.LastKnownPosition.ToPosition(),
                HasLastKnownPosition = entity.hasLastKnownPosition
            };
        }

        public static IEnumerable<OwnedEntity> ToModels(this IEnumerable<OwnedEntityData> entities)
        {
            return entities.Select(i => i.ToModel());
        }
    }
}
