using NSwag.Annotations;
using SdtdServerKit.Managers;

namespace SdtdServerKit.WebApi.Controllers.Settings
{
    /// <summary>
    /// 游戏商店配置
    /// </summary>
    [Authorize]
    [RoutePrefix("api/Settings/GameStore")]
    [OpenApiTag("Settings", Description = "配置")]
    public class GameStoreController : ApiController
    {
        /// <summary>
        /// 获取配置
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("")]
        public GameStoreSettings GetSettings()
        {
            var data = ConfigManager.Get<GameStoreSettings>();
            return data;
        }

        /// <summary>
        /// 更新配置
        /// </summary>
        /// <returns></returns>
        [HttpPut]
        [Route("")]
        public IHttpActionResult UpdateSettings([FromBody] GameStoreSettings model)
        {
            ConfigManager.Update(model);
            return Ok();
        }

        /// <summary>
        /// 重置配置
        /// </summary>
        /// <returns></returns>
        [HttpDelete]
        [Route("")]
        public GameStoreSettings ResetSettings()
        {
            var data = ConfigManager.LoadDefault<GameStoreSettings>();
            return data;
        }
    }
}