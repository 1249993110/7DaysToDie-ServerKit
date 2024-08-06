using SdtdServerKit.Managers;

namespace SdtdServerKit.WebApi.Controllers
{
    /// <summary>
    /// Players
    /// </summary>
    [Authorize]
    [RoutePrefix("api")]
    public class PlayersController : ApiController
    {
        #region OnlinePlayers
        /// <summary>
        /// 获取所有在线玩家
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("OnlinePlayers")]
        public IEnumerable<OnlinePlayer> GetOnlinePlayers()
        {
            return OnlinePlayerManager.GetAll();
        }

        /// <summary>
        /// 获取指定在线玩家
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("OnlinePlayers/{entityId:int}")]
        [ResponseType(typeof(OnlinePlayer))]
        public IHttpActionResult GetOnlinePlayer(int entityId)
        {
            if (OnlinePlayerManager.TryGetByEntityId(entityId, out var onlinePlayer))
            {
                return Ok(onlinePlayer); 
            }

            return NotFound();
        }

        /// <summary>
        /// 获取指定在线玩家
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("OnlinePlayers/{playerId}")]
        [ResponseType(typeof(OnlinePlayer))]
        public IHttpActionResult GetOnlinePlayer(string playerId)
        {
            if (OnlinePlayerManager.TryGetByPlayerId(playerId, out var onlinePlayer))
            {
                return Ok(onlinePlayer);
            }

            return NotFound();
        }

        #endregion

        #region HistoryPlayers
        /// <summary>
        /// 分页获取历史玩家
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("HistoryPlayers")]
        public Paged<HistoryPlayer> GetHistoryPlayers([FromUri]PaginationQuery<HistoryPlayerQueryOrder> model)
        {
            var persistentPlayerMap = GameManager.Instance.persistentPlayers.Players;
            int total = persistentPlayerMap.Count;
            IEnumerable<PersistentPlayerData> persistentPlayers = persistentPlayerMap.Values;

            string? keyword = model.Keyword;
            if (string.IsNullOrEmpty(keyword) == false)
            {
                total = 0;
                var filterByKeyword = new List<PersistentPlayerData>();
                foreach (var item in persistentPlayerMap.Values)
                {
                    if (string.Equals(item.PrimaryId.CombinedString, keyword, StringComparison.OrdinalIgnoreCase) 
                        || string.Equals(item.NativeId.CombinedString, keyword, StringComparison.OrdinalIgnoreCase)
                        || item.PlayerName.DisplayName.IndexOf(keyword, StringComparison.OrdinalIgnoreCase) != -1)
                    {
                        total += 1;
                        filterByKeyword.Add(item);
                    }
                }

                persistentPlayers = filterByKeyword;
            }

            if (model.Order == HistoryPlayerQueryOrder.LastLogin)
            {
                persistentPlayers = model.Desc ? persistentPlayers.OrderByDescending(k => k.LastLogin) : persistentPlayers.OrderBy(k => k.LastLogin);
            }
            else if(model.Order == HistoryPlayerQueryOrder.PlayerName)
            {
                persistentPlayers = model.Desc ? persistentPlayers.OrderByDescending(k => k.PlayerName) : persistentPlayers.OrderBy(k => k.PlayerName);
            }
            
            int pageSize = model.PageSize;
            if(pageSize > 0)
            {
                persistentPlayers = persistentPlayers.Skip((model.PageNumber - 1) * pageSize).Take(pageSize);
            }

            var historyPlayers = new List<HistoryPlayer>();
            foreach (var item in persistentPlayers)
            {
                historyPlayers.Add(new HistoryPlayer(item));
            }

            return new Paged<HistoryPlayer>(historyPlayers, total);
        }

        /// <summary>
        /// 获取指定历史玩家
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("HistoryPlayers/{playerId}")]
        [ResponseType(typeof(HistoryPlayer))]
        public IHttpActionResult GetHistoryPlayer(string playerId)
        {
            var userId = PlatformUserIdentifierAbs.FromCombinedString(playerId);
            if (userId == null)
            {
                return NotFound();
            }

            var persistentPlayerMap = GameManager.Instance.GetPersistentPlayerList().Players;
            if(persistentPlayerMap.TryGetValue(userId, out var persistentPlayerData) == false)
            {
                return NotFound();
            }

            var historyPlayer = new HistoryPlayer(persistentPlayerData);
            return Ok(historyPlayer);
        }

        #endregion
    }
}