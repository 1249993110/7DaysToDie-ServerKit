using SdtdServerKit.Managers;
using SdtdServerKit.Models;

namespace SdtdServerKit.WebApi.Controllers
{
    /// <summary>
    /// Player Skills
    /// </summary>
    [Authorize]
    [RoutePrefix("api/PlayerSkills")]
    public class PlayerSkillsController : ApiController
    {
        /// <summary>
        /// Gets player progression
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("{playerId}")]
        [ResponseType(typeof(IEnumerable<PlayerSkill>))]
        public IHttpActionResult Get(string playerId)
        {
            if (LivePlayerManager.TryGetByPlayerId(playerId, out var managedPlayer))
            {
                var progression = managedPlayer!.EntityPlayer.Progression;
                var result = new List<PlayerSkill>();

                //var attributesMap = Progression.ProgressionClasses.Where(i => i.Value.Type == ProgressionType.Attribute);
                foreach (var item in Progression.ProgressionClasses.Values)
                {
                    if (item.Type != ProgressionType.Attribute)
                    {
                        continue;
                    }
    
                    var progressionValue = progression.GetProgressionValue(item.Name);
                    var playerSkill = new PlayerSkill()
                    {
                        Name = item.Name,
                        LocalizationName = Utils.GetLocalization(item.NameKey, Language.Schinese, true),
                        LocalizationDesc = Utils.GetLocalization(item.DescKey, Language.Schinese, true),
                        //LocalizationLongDesc = Utils.GetLocalization(item.LongDescKey, Language.Schinese, true),
                        Level = progressionValue.Level,
                        MinLevel = item.MinLevel,
                        MaxLevel = item.MaxLevel,
                        CostForNextLevel = progressionValue.costForNextLevel,
                        Icon = item.Icon,
                        Type = item.Type.ToString(),
                        Children = GetChildren(progression, item),
                    };
                    result.Add(playerSkill);
                }

                return Ok(result);
            }

            return NotFound();
        }

        private static List<PlayerSkill> GetChildren(Progression progression, ProgressionClass parent)
        {
            var result = new List<PlayerSkill>();
            foreach (var child in Progression.ProgressionClasses.Values)
            {
                if (child.ParentName != null && child.ParentName == parent.Name)
                {
                    var childProgressionValue = progression.GetProgressionValue(child.Name);
                    var childPlayerSkill = new PlayerSkill()
                    {
                        Name = child.Name,
                        LocalizationName = Utils.GetLocalization(child.NameKey, Language.Schinese, true),
                        LocalizationDesc = Utils.GetLocalization(child.DescKey, Language.Schinese, true),
                        //LocalizationLongDesc = Utils.GetLocalization(child.LongDescKey, Language.Schinese, true),
                        Level = childProgressionValue.Level,
                        MinLevel = child.MinLevel,
                        MaxLevel = child.MaxLevel,
                        CostForNextLevel = childProgressionValue.costForNextLevel,
                        Icon = child.Icon,
                        Type = child.Type.ToString(),
                        Children = GetChildren(progression, child),
                    };
                    result.Add(childPlayerSkill);
                }
            }

            return result;
        }
    }
}
