using NSwag.Annotations;
using SdtdServerKit.FunctionSettings;
using SdtdServerKit.Managers;

namespace SdtdServerKit.WebApi.Controllers.Settings
{
    /// <summary>
    /// CdKey Redeem Settings Controller
    /// </summary>
    [Authorize]
    [RoutePrefix("api/Settings/CdKeyRedeem")]
    [OpenApiTag("Settings", Description = "配置")]
    public class CdKeyRedeemSettingsController : ApiController
    {
        /// <summary>
        /// 获取配置
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("")]
        public CdKeyRedeemSettings GetSettings([FromUri] Language language)
        {
            var data = ConfigManager.GetRequired<CdKeyRedeemSettings>(Locales.Get(language));
            return data;
        }

        /// <summary>
        /// 更新配置
        /// </summary>
        /// <returns></returns>
        [HttpPut]
        [Route("")]
        public IHttpActionResult UpdateSettings([FromBody] CdKeyRedeemSettings model)
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
        public CdKeyRedeemSettings ResetSettings([FromUri] Language language)
        {
            var data = ConfigManager.LoadDefault<CdKeyRedeemSettings>(Locales.Get(language));
            return data;
        }
    }
}