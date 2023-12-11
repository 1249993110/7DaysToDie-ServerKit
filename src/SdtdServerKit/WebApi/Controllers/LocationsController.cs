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
        public IEnumerable<EntityInfo> Get(Shared.Models.EntityType entityType = Shared.Models.EntityType.All)
        {
            var locations = new List<EntityInfo>();

            if (entityType == Shared.Models.EntityType.Player || entityType == Shared.Models.EntityType.All)
            {
                foreach (var players in GameManager.Instance.World.Players.list)
                {
                    locations.Add(new EntityInfo()
                    {
                        EntityId = players.entityId,
                        EntityName = players.EntityName,
                        Position = players.GetPosition().ToPosition(),
                        EntityType = Shared.Models.EntityType.Player,
                    });
                }
            }
            else if (entityType == Shared.Models.EntityType.Animal || entityType == Shared.Models.EntityType.All)
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
                            EntityType = Shared.Models.EntityType.Animal,
                        });
                    }
                }
            }
            else if (entityType == Shared.Models.EntityType.Hostiles || entityType == Shared.Models.EntityType.All)
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
                            EntityType = (Shared.Models.EntityType)entityEnemy.entityType
                        });
                    }
                }
            }
            else if (entityType == Shared.Models.EntityType.Zombie)
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
                            EntityType = (Shared.Models.EntityType)entityZombie.entityType
                        });
                    }
                }
            }
            else if (entityType == Shared.Models.EntityType.Bandit)
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
                            EntityType = (Shared.Models.EntityType)entityBandit.entityType
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
                    EntityType = Shared.Models.EntityType.Player,
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
                    EntityType = Shared.Models.EntityType.Player,
                });
            }

            return NotFound();
        }
    }
}
