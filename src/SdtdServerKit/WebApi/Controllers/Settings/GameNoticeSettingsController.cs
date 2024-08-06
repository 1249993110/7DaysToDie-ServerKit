using NSwag.Annotations;
using SdtdServerKit.FunctionSettings;
using SdtdServerKit.Managers;

namespace SdtdServerKit.WebApi.Controllers.Settings
{
    /// <summary>
    /// 游戏公告配置
    /// </summary>
    [Authorize]
    [RoutePrefix("api/Settings/GameNotice")]
    [OpenApiTag("Settings", Description = "配置")]
    public class GameNoticeSettingsController : ApiController
    {
        /// <summary>
        /// 获取配置
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("")]
        public GameNoticeSettings GetSettings()
        {
            var data = ConfigManager.Get<GameNoticeSettings>();
            return data;
        }

        /// <summary>
        /// 更新配置
        /// </summary>
        /// <returns></returns>
        [HttpPut]
        [Route("")]
        public IHttpActionResult UpdateSettings([FromBody] GameNoticeSettings model)
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
        public GameNoticeSettings ResetSettings()
        {
            var data = ConfigManager.LoadDefault<GameNoticeSettings>();
            return data;
        }
    }
}