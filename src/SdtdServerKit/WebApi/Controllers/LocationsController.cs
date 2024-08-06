using Webserver.WebAPI.APIs.WorldState;

namespace SdtdServerKit.WebApi.Controllers
{
    /// <summary>
    /// Locations
    /// </summary>
    [Authorize]
    [RoutePrefix("api/Locations")]
    public class LocationsController : ApiController
    {
        /// <summary>
        /// 获取位置
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("")]
        public IEnumerable<EntityInfo> Get(Models.EntityType entityType)
        {
            var locations = new List<EntityInfo>();

            if (entityType == Models.EntityType.OfflinePlayer)
            {
                var online = GameManager.Instance.World.Players.list.Select(i => ConnectionManager.Instance.Clients.ForEntityId(i.entityId).InternalId).ToHashSet();
                foreach (var item in GameManager.Instance.GetPersistentPlayerList().Players)
                {
                    if(online.Contains(item.Key) == false)
                    {
                        var player = item.Value;
                        locations.Add(new EntityInfoEx()
                        {
                            EntityId = player.EntityId,
                            EntityName = player.PlayerName.DisplayName,
                            Position = player.Position.ToPosition(),
                            EntityType = Models.EntityType.OfflinePlayer,
                            PlayerId = player.PrimaryId.CombinedString,
                        });
                    }
                }
            }
            else if (entityType == Models.EntityType.OnlinePlayer)
            {
                foreach (var player in GameManager.Instance.World.Players.list)
                {
                    locations.Add(new EntityInfoEx()
                    {
                        EntityId = player.entityId,
                        EntityName = player.EntityName,
                        Position = player.GetPosition().ToPosition(),
                        EntityType = Models.EntityType.OnlinePlayer,
                        PlayerId = ConnectionManager.Instance.Clients.ForEntityId(player.entityId).InternalId.CombinedString,
                    });
                }
            }
            else if (entityType == Models.EntityType.Animal)
            {
                foreach (var entity in GameManager.Instance.World.Entities.list)
                {
                    if (entity is EntityAnimal entityAnimal && entity.IsAlive())
                    {
                        locations.Add(new EntityInfo()
                        {
                            EntityId = entityAnimal.entityId,
                            EntityName = entityAnimal.EntityName ?? ("animal class #" + entityAnimal.entityClass),
                            Position = entityAnimal.GetPosition().ToPosition(),
                            EntityType = Models.EntityType.Animal,
                        });
                    }
                }
            }
            else if (entityType == Models.EntityType.Hostiles)
            {
                foreach (var entity in GameManager.Instance.World.Entities.list)
                {
                    if (entity is EntityEnemy entityEnemy && entity.IsAlive())
                    {
                        locations.Add(new EntityInfo()
                        {
                            EntityId = entityEnemy.entityId,
                            EntityName = entityEnemy.EntityName ?? ("enemy class #" + entityEnemy.entityClass),
                            Position = entityEnemy.GetPosition().ToPosition(),
                            EntityType = (Models.EntityType)entityEnemy.entityType
                        });
                    }
                }
            }
            else if (entityType == Models.EntityType.Zombie)
            {
                foreach (var entity in GameManager.Instance.World.Entities.list)
                {
                    if (entity is EntityZombie entityZombie && entity.IsAlive())
                    {
                        locations.Add(new EntityInfo()
                        {
                            EntityId = entityZombie.entityId,
                            EntityName = entityZombie.EntityName ?? ("zombie class #" + entityZombie.entityClass),
                            Position = entityZombie.GetPosition().ToPosition(),
                            EntityType = (Models.EntityType)entityZombie.entityType
                        });
                    }
                }
            }
            else if (entityType == Models.EntityType.Bandit)
            {
                foreach (var entity in GameManager.Instance.World.Entities.list)
                {
                    if (entity is EntityBandit entityBandit && entity.IsAlive())
                    {
                        locations.Add(new EntityInfo()
                        {
                            EntityId = entityBandit.entityId,
                            EntityName = entityBandit.EntityName ?? ("bandit class #" + entityBandit.entityClass),
                            Position = entityBandit.GetPosition().ToPosition(),
                            EntityType = (Models.EntityType)entityBandit.entityType
                        });
                    }
                }
            }

            return locations;
        }

        /// <summary>
        /// 获取位置
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("{entityId:int}")]
        [ResponseType(typeof(EntityInfo))]
        public IHttpActionResult Get(int entityId)
        {
            if(GameManager.Instance.World.Players.dict.TryGetValue(entityId, out var player))
            {
                return Ok(new EntityInfo()
                {
                    EntityId = player.entityId,
                    EntityName = player.EntityName,
                    Position = player.GetPosition().ToPosition(),
                    EntityType = Models.EntityType.OnlinePlayer,
                });
            }

            if (GameManager.Instance.World.Entities.dict.TryGetValue(entityId, out var entity))
            {
                string entityName = (entity is EntityAlive entityAlive) ? entityAlive.EntityName : "entity class #" + entity.entityClass;
                return Ok(new EntityInfo()
                {
                    EntityId = entity.entityId,
                    EntityName = entityName,
                    Position = entity.GetPosition().ToPosition(),
                    EntityType = (Models.EntityType)entity.entityType,
                });
            }

            return NotFound();
        }
    }
}
