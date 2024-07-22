using NSwag.Annotations;
using SdtdServerKit.Managers;

namespace SdtdServerKit.WebApi.Controllers.Settings
{
    /// <summary>
    /// VIP礼包配置
    /// </summary>
    [Authorize]
    [RoutePrefix("api/Settings/VipGift")]
    [OpenApiTag("Settings", Description = "配置")]
    public class VipGiftSettingsController : ApiController
    {
        /// <summary>
        /// 获取配置
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("")]
        public VipGiftSettings GetSettings()
        {
            var data = ConfigManager.Get<VipGiftSettings>();
            return data;
        }

        /// <summary>
        /// 更新配置
        /// </summary>
        /// <returns></returns>
        [HttpPut]
        [Route("")]
        public IHttpActionResult UpdateSettings([FromBody] VipGiftSettings model)
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
        public VipGiftSettings ResetSettings()
        {
            var data = ConfigManager.LoadDefault<VipGiftSettings>();
            return data;
        }
    }
}