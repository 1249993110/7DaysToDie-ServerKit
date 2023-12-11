using SkiaSharp;

namespace SdtdServerKit.WebApi.Controllers
{
    /// <summary>
    /// ItemIcons
    /// </summary>
    public class ItemIconsController : ApiController
    {
        /// <summary>
        /// 获取 ItemIcon
        /// </summary>
        /// <remarks>
        /// e.g. /api/ItemIcons/airConditioner__00FF00.png 颜色是可选的
        /// </remarks>
        /// <param name="iconName">图标名称, 可带颜色, 格式见例子</param>
        /// <returns></returns>
        [HttpGet]
        [ResponseCache(Duration = 7200)]
        [Route("api/ItemIcons/{iconName}")]
        public IHttpActionResult Get(string iconName)
        {
            if (iconName.EndsWith(".png", StringComparison.OrdinalIgnoreCase) == false)
            {
                return BadRequest("Invalid icon name.");
            }

            int len = iconName.Length;
            string? iconColor = null;
            if (len > 12 && iconName[len - 11] == '_' && iconName[len - 12] == '_')
            {
                iconColor = iconName.Substring(len - 10, 6);
                iconName = string.Concat(iconName.Substring(0, len - 12), ".png");
            }

            string? iconPath = FindIconPath(iconName);
            if (iconPath == null)
            {
                return NotFound();
            }

            byte[] data = System.IO.File.ReadAllBytes(iconPath);
            if (iconColor == null)
            {
                return new FileContentResult(data, "image/png");
            }
            else
            {
                int r, g, b;
                try
                {
                    r = Convert.ToInt32(iconColor.Substring(0, 2), 16);
                    g = Convert.ToInt32(iconColor.Substring(2, 2), 16);
                    b = Convert.ToInt32(iconColor.Substring(4, 2), 16);
                }
                catch
                {
                    return BadRequest("Invalid icon color.");
                }

                using var skBitmap = SKBitmap.Decode(data);
                int width = skBitmap.Width;
                int height = skBitmap.Height;

                for (int i = 0; i < width; i++)
                {
                    for (int j = 0; j < height; j++)
                    {
                        var skColor = skBitmap.GetPixel(i, j);

                        skBitmap.SetPixel(i, j, new SKColor(
                            (byte)(skColor.Red * r / 255),
                            (byte)(skColor.Green * g / 255),
                            (byte)(skColor.Blue * b / 255),
                            skColor.Alpha));
                    }
                }

                var stream = new MemoryStream(data.Length / 2);
                skBitmap.Encode(stream, SKEncodedImageFormat.Png, 100);
                stream.Position = 0L;
                return new FileStreamResult(stream, "image/png");
            }
        }

        private static string? FindIconPath(string iconName)
        {
            string path = "Data/ItemIcons/" + iconName;
            if (File.Exists(path))
            {
                return path;
            }

            foreach (Mod mod in ModManager.GetLoadedMods())
            {
                var di = new DirectoryInfo(mod.Path);
                var files = di.GetFiles(iconName, SearchOption.AllDirectories);

                if (files.Length > 0)
                {
                    return files[0].FullName;
                }
            }

            return null;
        }
    }
}
