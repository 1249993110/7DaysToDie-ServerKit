using System.ComponentModel.DataAnnotations;

namespace SdtdServerKit.WebApi.Controllers
{
    /// <summary>
    /// Permissions
    /// </summary>
    [Authorize]
    [RoutePrefix("api/Permissions")]
    public class PermissionsController : ApiController
    {
        /// <summary>
        /// Add permissions
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("")]
        public IEnumerable<string> AddPermissions([FromBody, Required, MinLength(1)] PermissionEntry[] permissions)
        {
            var executeResult = new List<string>();
            foreach (var item in permissions)
            {
                string command = $"cp add {item.Command} {item.Level}";
                var result = SdtdConsole.Instance.ExecuteSync(command, ModApi.CmdExecuteDelegate);
                executeResult.AddRange(result);
            }
            
            return executeResult;
        }

        /// <summary>
        /// Get permissions
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("")]
        public IEnumerable<PermissionEntry> GetPermissions()
        {
            var permissions = new List<PermissionEntry>();
            foreach (var item in GameManager.Instance.adminTools.Commands.GetCommands().Values)
            {
                permissions.Add(new PermissionEntry()
                {
                    Command = item.Command,
                    Level = item.PermissionLevel
                });
            }

            return permissions;
        }

        /// <summary>
        /// Remove permissions
        /// </summary>
        /// <param name="playerIds"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route("")]
        public IEnumerable<string> RemovePermissions([FromUri, Required, MinLength(1)] string[] playerIds)
        {
            var executeResult = new List<string>();
            foreach (var item in playerIds)
            {
                string command = $"cp remove {item}";
                var result = SdtdConsole.Instance.ExecuteSync(command, ModApi.CmdExecuteDelegate);
                executeResult.AddRange(result);
            }

            return executeResult;
        }
    }
}
