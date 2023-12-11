namespace SdtdServerKit.WebApi.Controllers
{
    /// <summary>
    /// Land Claims
    /// </summary>
    [RoutePrefix("api/LandClaims")]
    public class LandClaimsController : ApiController
    {
        /// <summary>
        /// 获取指定玩家的领地石
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("{playerId}")]
        [ResponseType(typeof(ClaimOwner))]
        public IHttpActionResult GetLandClaim(string playerId)
        {
            var userId = PlatformUserIdentifierAbs.FromCombinedString(playerId);
            if (userId == null)
            {
                return NotFound();
            }

            var persistentPlayerData = GameManager.Instance.GetPersistentPlayerList().GetPlayerData(userId);
            if (persistentPlayerData == null)
            {
                return NotFound();
            }

            var claimOwner = new ClaimOwner()
            {
                PlatformId = persistentPlayerData.PlatformUserIdentifier.CombinedString,
                CrossplatformId = persistentPlayerData.UserIdentifier.CombinedString,
                PlayerName = persistentPlayerData.PlayerName,
                ClaimActive = GameManager.Instance.World.IsLandProtectionValidForPlayer(persistentPlayerData),
                ClaimPositions = persistentPlayerData.LPBlocks.ToPositions(),
            };

            return Ok(claimOwner);
        }

        /// <summary>
        /// 获取所有玩家的领地石
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("")]
        public LandClaims GetLandClaims()
        {
            var claimOwners = new List<ClaimOwner>();

            foreach (var item in GameManager.Instance.GetPersistentPlayerList().Players)
            {
                var persistentPlayerData = item.Value;
                if(persistentPlayerData != null)
                {
                    claimOwners.Add(new ClaimOwner()
                    {
                        PlatformId = persistentPlayerData.UserIdentifier.CombinedString,
                        CrossplatformId = persistentPlayerData.PlatformUserIdentifier.CombinedString,
                        PlayerName = persistentPlayerData.PlayerName,
                        ClaimActive = GameManager.Instance.World.IsLandProtectionValidForPlayer(persistentPlayerData),
                        ClaimPositions = persistentPlayerData.LPBlocks.ToPositions(),
                    });
                }
            }

            return new LandClaims()
            {
                ClaimOwners = claimOwners,
                ClaimSize = GamePrefs.GetInt(EnumGamePrefs.LandClaimSize)
            };
        }
    }
}
