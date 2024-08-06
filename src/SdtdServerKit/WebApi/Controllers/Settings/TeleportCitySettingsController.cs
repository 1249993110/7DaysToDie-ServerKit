﻿using NSwag.Annotations;
using SdtdServerKit.FunctionSettings;
using SdtdServerKit.Managers;

namespace SdtdServerKit.WebApi.Controllers.Settings
{
    /// <summary>
    /// 公共回城配置
    /// </summary>
    [Authorize]
    [RoutePrefix("api/Settings/TeleportCity")]
    [OpenApiTag("Settings", Description = "配置")]
    public class TeleportCitySettingsController : ApiController
    {
        /// <summary>
        /// 获取配置
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("")]
        public TeleportCitySettings GetSettings()
        {
            var data = ConfigManager.Get<TeleportCitySettings>();
            return data;
        }

        /// <summary>
        /// 更新配置
        /// </summary>
        /// <returns></returns>
        [HttpPut]
        [Route("")]
        public IHttpActionResult UpdateSettings([FromBody] TeleportCitySettings model)
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
        public TeleportCitySettings ResetSettings()
        {
            var data = ConfigManager.LoadDefault<TeleportCitySettings>();
            return data;
        }
    }
}