using Webserver.WebAPI.APIs.WorldState;

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
            var onlinePlayers = new List<OnlinePlayer>();
            foreach (var clientInfo in ConnectionManager.Instance.Clients.List)
            {
                if(clientInfo.bAttachedToEntity)
                {
                    onlinePlayers.Add(clientInfo.ToOnlinePlayer());
                }
            }

            return onlinePlayers;
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
            var clientInfo = ConnectionManager.Instance.Clients.ForEntityId(entityId);
            if (clientInfo == null || clientInfo.bAttachedToEntity == false)
            {
                return NotFound();
            }

            return Ok(clientInfo.ToOnlinePlayer());
        }

        /// <summary>
        /// 获取指定在线玩家
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("OnlinePlayers/{playerIdOrName}")]
        [ResponseType(typeof(OnlinePlayer))]
        public IHttpActionResult GetOnlinePlayer(string playerIdOrName)
        {
            var clientInfo = ConsoleHelper.ParseParamIdOrName(playerIdOrName);
            if (clientInfo == null || clientInfo.bAttachedToEntity == false)
            {
                return NotFound();
            }

            return Ok(clientInfo.ToOnlinePlayer());
        }

        /// <summary>
        /// 获取指定在线玩家细节
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("OnlinePlayers/{playerId}/Details")]
        [ResponseType(typeof(OnlinePlayerDetails))]
        public IHttpActionResult GetOnlinePlayerDetails(string playerId)
        {
            var userId = PlatformUserIdentifierAbs.FromCombinedString(playerId);
            if (userId == null)
            {
                return NotFound();
            }

            var clientInfo = ConnectionManager.Instance.Clients.ForUserId(userId);
            if (clientInfo == null || clientInfo.bAttachedToEntity == false)
            {
                return NotFound();
            }

            return Ok(clientInfo.ToOnlinePlayerDetails());
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
            var persistentPlayersDict = GameManager.Instance.GetPersistentPlayerList().Players;
            int total = persistentPlayersDict.Count;
            IEnumerable<PersistentPlayerData> persistentPlayers = persistentPlayersDict.Values;

            string? keyword = model.Keyword;
            if (string.IsNullOrEmpty(keyword) == false)
            {
                total = 0;
                var filterByKeyword = new List<PersistentPlayerData>();
                foreach (var item in persistentPlayersDict.Values)
                {
                    if (string.Equals(item.UserIdentifier.CombinedString, keyword, StringComparison.OrdinalIgnoreCase) 
                        || string.Equals(item.PlatformUserIdentifier.CombinedString, keyword, StringComparison.OrdinalIgnoreCase)
                        || item.PlayerName.IndexOf(keyword, StringComparison.OrdinalIgnoreCase) > 0)
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
                historyPlayers.Add(item.ToHistoryPlayer());
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

            var persistentPlayersDict = GameManager.Instance.GetPersistentPlayerList().Players;
            if(persistentPlayersDict.TryGetValue(userId, out var persistentPlayerData) == false)
            {
                return NotFound();
            }

            var historyPlayer = persistentPlayerData.ToHistoryPlayer();
            return Ok(historyPlayer);
        }

        /// <summary>
        /// 获取指定历史玩家细节
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("HistoryPlayers/{playerId}/Details")]
        [ResponseType(typeof(HistoryPlayerDetails))]
        public IHttpActionResult GetHistoryPlayerDetails(string playerId)
        {
            var userId = PlatformUserIdentifierAbs.FromCombinedString(playerId);
            if (userId == null)
            {
                return NotFound();
            }

            var persistentPlayersDict = GameManager.Instance.GetPersistentPlayerList().Players;
            if (persistentPlayersDict.TryGetValue(userId, out var persistentPlayerData) == false)
            {
                return NotFound();
            }

            return Ok(persistentPlayerData.ToHistoryPlayerDetails());
        }
        #endregion
    }
}