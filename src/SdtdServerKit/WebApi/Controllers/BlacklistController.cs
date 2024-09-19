using System.ComponentModel.DataAnnotations;

namespace SdtdServerKit.WebApi.Controllers
{
    /// <summary>
    /// Blacklist
    /// </summary>
    [Authorize]
    [RoutePrefix("api/Blacklist")]
    public class BlacklistController : ApiController
    {
        /// <summary>
        /// Add blacklist
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("")]
        public IEnumerable<string> AddBlacklist([FromBody, Required, MinLength(1)] BlacklistEntry[] blacklist)
        {
            var executeResult = new List<string>();
            foreach (var item in blacklist)
            {
                string command = $"ban add {item.PlayerId} {(int)(item.BannedUntil - DateTime.Now).TotalMinutes} minutes {Utilities.Utils.FormatCommandArgs(item.Reason)} {Utilities.Utils.FormatCommandArgs(item.DisplayName)}";
                var result = SdtdConsole.Instance.ExecuteSync(command, ModApi.CmdExecuteDelegate);
                executeResult.AddRange(result);
            }
            
            return executeResult;
        }

        /// <summary>
        /// Get blacklist
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("")]
        public IEnumerable<BlacklistEntry> GetBlacklist()
        {
            var blacklist = new List<BlacklistEntry>();
            foreach (var item in GameManager.Instance.adminTools.Blacklist.GetBanned())
            {
                blacklist.Add(new BlacklistEntry()
                {
                    PlayerId = item.UserIdentifier.CombinedString,
                    BannedUntil = item.BannedUntil,
                    Reason = item.BanReason,
                    DisplayName = item.Name
                });
            }

            return blacklist;
        }

        /// <summary>
        /// Remove blacklist
        /// </summary>
        /// <param name="playerIds"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route("")]
        public IEnumerable<string> RemoveBlacklist([FromUri, Required, MinLength(1)] string[] playerIds)
        {
            var executeResult = new List<string>();
            foreach (var item in playerIds)
            {
                string command = $"ban remove {item}";
                var result = SdtdConsole.Instance.ExecuteSync(command, ModApi.CmdExecuteDelegate);
                executeResult.AddRange(result);
            }

            return executeResult;
        }
    }
}
