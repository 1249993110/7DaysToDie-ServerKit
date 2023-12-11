namespace SdtdServerKit.WebApi.Controllers
{
    /// <summary>
    /// System Info
    /// </summary>
    [Authorize]
    [RoutePrefix("api/SystemInfo")]
    public class SystemInfoController : ApiController
    {
        /// <summary>
        ///系统信息
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("")]
        public SystemInfo SystemInfo()
        {
            return new SystemInfo() 
            {
                DeviceModel = UnityEngine.Device.SystemInfo.deviceModel,
                DeviceName = UnityEngine.Device.SystemInfo.deviceName,
                DeviceType = UnityEngine.Device.SystemInfo.deviceType.ToString(),
                DeviceUniqueIdentifier = UnityEngine.Device.SystemInfo.deviceUniqueIdentifier,
                OperatingSystem = UnityEngine.Device.SystemInfo.operatingSystem,
                OperatingSystemFamily = UnityEngine.Device.SystemInfo.operatingSystemFamily.ToString(),
                ProcessorCount = UnityEngine.Device.SystemInfo.processorCount,
                ProcessorFrequency = UnityEngine.Device.SystemInfo.processorFrequency,
                ProcessorType = UnityEngine.Device.SystemInfo.processorType,
                SystemMemorySize = UnityEngine.Device.SystemInfo.systemMemorySize,
            };
        }       
    }
}
