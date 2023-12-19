namespace SdtdServerKit.WebApi.Controllers
{
    /// <summary>
    /// Localization
    /// </summary>
    [Authorize]
    [RoutePrefix("api/Localization")]
    [ResponseCache(Duration = 7200)]
    public class LocalizationController : ApiController
    {
        /// <summary>
        /// 获取本地化字典
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("")]
        [ResponseType(typeof(Dictionary<string, string>))]
        public IHttpActionResult GetLocalization(Language language)
        {
            string _language = language.ToString().ToLower();
            var dict = Localization.dictionary;
            int languageIndex = Array.LastIndexOf(dict["KEY"], _language);

            if (languageIndex < 0)
            {
                return NotFound();
            }

            return Ok(dict.ToDictionary(p => p.Key, p => p.Value[languageIndex]));
        }

        /// <summary>
        /// 获取指定项目的本地化字符串
        /// </summary>
        /// <param name="itemName">项目名称</param>
        /// <param name="language">语言</param>
        /// <returns></returns>
        [HttpGet]
        [Route("{itemName}")]
        [ResponseType(typeof(string))]
        public IHttpActionResult GetLocalization(string itemName, Language language)
        {
            string _language = language.ToString().ToLower();

            var dict = Localization.dictionary;
            int languageIndex = Array.LastIndexOf(dict["KEY"], _language);

            if (languageIndex < 0 || dict.ContainsKey(itemName) == false)
            {
                return NotFound();
            }

            return Ok(dict[itemName][languageIndex]);
        }

        /// <summary>
        /// 获取已知的语言
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("~/api/KnownLanguages")]
        [ResponseType(typeof(string))]
        public string[] GetKnownLanguages()
        {
            return Localization.dictionary["KEY"];
        }
    }
}
