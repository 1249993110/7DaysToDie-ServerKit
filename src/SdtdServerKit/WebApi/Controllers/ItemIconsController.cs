﻿using SkiaSharp;

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
        /// <param name="name">可以是图标名称也可以是物品名称, 为图标名称时后缀必须为.png 可带颜色, 格式见例子</param>
        /// <returns></returns>
        [HttpGet]
        [ResponseCache(Duration = 7200)]
        [Route("api/ItemIcons/{name}")]
        public IHttpActionResult Get(string name)
        {
            if (name.EndsWith(".png", StringComparison.OrdinalIgnoreCase) == false)
            {
                var itemClass = ItemClass.GetItemClass(name);
                if (itemClass == null)
                {
                    return BadRequest("Invalid item name.");
                }

                string iconFileName = itemClass.GetIconName() + ".png";
                var iconColor = itemClass.GetIconTint();

                if(iconColor == UnityEngine.Color.white)
                {
                    return InternalGet(iconFileName);
                }

                return InternalGet(iconFileName, (int)iconColor.r, (int)iconColor.g, (int)iconColor.b);
            }
            else
            {
                int len = name.Length;
                if (len > 12 && name[len - 11] == '_' && name[len - 12] == '_')
                {
                    string iconColor = name.Substring(len - 10, 6);
                    string iconFileName = string.Concat(name.Substring(0, len - 12), ".png");

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

                    return InternalGet(iconFileName, r, g, b);
                }

                return InternalGet(name);
            }
        }

        private IHttpActionResult InternalGet(string iconFileName)
        {
            string? iconPath = FindIconPath(iconFileName);
            if (iconPath == null)
            {
                return NotFound();
            }

            byte[] data = System.IO.File.ReadAllBytes(iconPath);
            return new FileContentResult(data, "image/png");
        }

        private IHttpActionResult InternalGet(string iconFileName, int r, int g, int b)
        {
            string? iconPath = FindIconPath(iconFileName);
            if (iconPath == null)
            {
                return NotFound();
            }

            byte[] data = System.IO.File.ReadAllBytes(iconPath);
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

        private static string? FindIconPath(string icon)
        {
            string path = "Data/ItemIcons/" + icon;
            if (File.Exists(path))
            {
                return path;
            }

            foreach (Mod mod in ModManager.GetLoadedMods())
            {
                var di = new DirectoryInfo(mod.Path);
                var files = di.GetFiles(icon, SearchOption.AllDirectories);

                if (files.Length > 0)
                {
                    return files[0].FullName;
                }
            }

            return null;
        }
    }
}
