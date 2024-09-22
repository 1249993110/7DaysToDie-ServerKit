using MapRendering;

namespace SdtdServerKit.WebApi.Controllers
{
    /// <summary>
    /// 地图
    /// </summary>
    // [Authorize]
    [RoutePrefix("api/Map")]
    public class MapController : ApiController
    {
        /// <summary>
        /// 获取地图信息
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("Info")]
        public MapInfo MapInfo()
        {
            var mapInfo = new MapInfo() 
            { 
                BlockSize = MapRendering.Constants.MapBlockSize, 
                MaxZoom = MapRendering.Constants.Zoomlevels - 1 
            };
            return mapInfo;
        }

        /// <summary>
        /// 获取切片
        /// </summary>
        /// <param name="z">zoom</param>
        /// <param name="x"></param>
        /// <param name="y">y</param>
        /// <returns></returns>
        [HttpGet]
        [Route("Tile/{z:int}/{x:int}/{y:int}")]
        public IHttpActionResult MapTile(int z, int x, int y)
        {
            string fileName = MapRendering.Constants.MapDirectory + $"/{z}/{x}/{y}.png";
            
            if (File.Exists(fileName))
            {
                return new FileStreamResult(File.OpenRead(fileName), "image/png");
            }

            if(ModApi.MapTileCache == null)
            {
                return NotFound();
            }

            byte[] data = ModApi.MapTileCache.GetFileContent(fileName);
            return new FileContentResult(data, "image/png");
        }

        /// <summary>
        /// 渲染整个地图
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("RenderFullMap")]
        public IHttpActionResult RenderFullMap()
        {
            var result = Utilities.Utils.ExecuteConsoleCommand("visitmap full");
            return Ok(result);
        }

        /// <summary>
        /// 渲染已探索区域
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("RenderExploredArea")]
        public IHttpActionResult RenderExploredArea()
        {
            ModApi.MainThreadSyncContext.Post((state) =>
            {
                MapRenderer.Instance.RenderFullMap();
            }, null);

            return Ok();
        }
    }
}
