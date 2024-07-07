﻿using NSwag.Annotations;
using SdtdServerKit.Managers;

namespace SdtdServerKit.WebApi.Controllers.Settings
{
    /// <summary>
    /// 私人回城配置
    /// </summary>
    [Authorize]
    [RoutePrefix("api/Settings/TeleportHome")]
    [OpenApiTag("Settings", Description = "配置")]
    public class TeleportHomeSettingsController : ApiController
    {
        /// <summary>
        /// 获取配置
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("")]
        public TeleportHomeSettings GetSettings()
        {
            var data = ConfigManager.Get<TeleportHomeSettings>();
            return data;
        }

        /// <summary>
        /// 更新配置
        /// </summary>
        /// <returns></returns>
        [HttpPut]
        [Route("")]
        public IHttpActionResult UpdateSettings([FromBody] TeleportHomeSettings model)
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
        public TeleportHomeSettings ResetSettings()
        {
            var data = ConfigManager.LoadDefault<TeleportHomeSettings>();
            return data;
        }
    }
}