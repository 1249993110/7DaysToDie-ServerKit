using NSwag.Annotations;
using SdtdServerKit.Managers;

namespace SdtdServerKit.WebApi.Controllers.Settings
{
    /// <summary>
    /// 全局配置
    /// </summary>
    [Authorize]
    [RoutePrefix("api/Settings/GlobalSettings")]
    [OpenApiTag("Settings", Description = "配置")]
    public class GlobalSettingsController : ApiController
    {
        /// <summary>
        /// 获取配置
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("")]
        public GlobalSettings GetSettings()
        {
            var data = ConfigManager.Get<GlobalSettings>();
            return data;
        }

        /// <summary>
        /// 更新配置
        /// </summary>
        /// <returns></returns>
        [HttpPut]
        [Route("")]
        public IHttpActionResult UpdateSettings([FromBody] GlobalSettings model)
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
        public GlobalSettings ResetSettings()
        {
            var data = ConfigManager.LoadDefault<GlobalSettings>();
            return data;
        }
    }
}