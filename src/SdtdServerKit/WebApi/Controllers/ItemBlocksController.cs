namespace SdtdServerKit.WebApi.Controllers
{
    /// <summary>
    /// Item and Blocks
    /// </summary>
    [Authorize]
    [RoutePrefix("api/ItemBlocks")]
    public class ItemBlocksController : ApiController
    {
        /// <summary>
        /// Get item and blocks.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("")]
        public Paged<ItemBlock> GetItemBlocks([FromUri] ItemBlockQuery model)
        {
            List<ItemBlock> itemBlocks;
            string language = model.Language.ToString().ToLower();
            switch (model.ItemBlockKind)
            {
                case ItemBlockKind.All:
                    itemBlocks = GetAllItemsAndBlocks(language, model.Keyword, model.ShowUserHidden);
                    break;

                case ItemBlockKind.Item:
                    itemBlocks = GetAllItems(language, model.Keyword, model.ShowUserHidden);
                    break;

                case ItemBlockKind.Block:
                    itemBlocks = GetAllBlocks(language, model.Keyword, model.ShowUserHidden);
                    break;
                default:
                    itemBlocks = new List<ItemBlock>();
                    break;
            }

            int pageSize = model.PageSize;
            var items = pageSize < 0 ? itemBlocks : itemBlocks.Skip((model.PageNumber - 1) * pageSize).Take(pageSize);
            var result = new Paged<ItemBlock>(items, itemBlocks.Count);

            return result;
        }

        private static List<Models.ItemBlock> GetAllBlocks(string language, string? keyword, bool showUserHidden)
        {
            return GetItemsAndBlocks(0, Block.ItemsStartHere, item => item.IsBlock() == true, language, keyword, showUserHidden);
        }

        private static List<Models.ItemBlock> GetAllItems(string language, string? keyword, bool showUserHidden)
        {
            return GetItemsAndBlocks(Block.ItemsStartHere, ItemClass.list.Length, item => item.IsBlock() == false, language, keyword, showUserHidden);
        }

        private static List<Models.ItemBlock> GetAllItemsAndBlocks(string language, string? keyword, bool showUserHidden)
        {
            return GetItemsAndBlocks(0, ItemClass.list.Length, null, language, keyword, showUserHidden);
        }

        private static List<Models.ItemBlock> GetItemsAndBlocks(
            int startId,
            int endId,
            Func<ItemClass, bool>? filter,
            string language,
            string? keyword,
            bool showUserHidden)
        {
            var dict = Localization.dictionary;
            int languageIndex = Array.LastIndexOf(dict["KEY"], language);

            if (languageIndex < 0)
            {
                throw new Exception($"The specified language: {language} does not exist");
            }

            var result = new List<Models.ItemBlock>();
            for (int id = startId; id < endId; id++)
            {
                ItemClass item = ItemClass.GetForId(id);
                if (item != null)
                {
                    EnumCreativeMode creativeMode = item.CreativeMode;
                    if (creativeMode != EnumCreativeMode.None
                        && creativeMode != EnumCreativeMode.Test
                        && (creativeMode == EnumCreativeMode.All || showUserHidden)
                        && (filter == null || filter.Invoke(item)))
                    {
                        string itemName = item.GetItemName();

                        if (string.IsNullOrEmpty(itemName))
                        {
                            continue;
                        }

                        string localizationName;
                        if (dict.ContainsKey(itemName) == false)
                        {
                            localizationName = itemName;
                        }
                        else
                        {
                            localizationName = dict[itemName][languageIndex] ?? itemName;
                        }

                        if (string.IsNullOrEmpty(keyword) == false)
                        {
                            if (itemName.IndexOf(keyword, StringComparison.OrdinalIgnoreCase) == -1
                                && localizationName.IndexOf(keyword, StringComparison.OrdinalIgnoreCase) == -1
                                && id.ToString().IndexOf(keyword, StringComparison.OrdinalIgnoreCase) == -1)
                            {
                                continue;
                            }
                        }

                        var itemBlock = new Models.ItemBlock()
                        {
                            Id = id,
                            ItemName = itemName,
                            IconColor = item.GetIconTint().ToHex(),
                            MaxStackAllowed = item.Stacknumber.Value,
                            IconName = item.GetIconName(),
                            IsBlock = item.IsBlock(),
                            LocalizationName = localizationName
                        };

                        result.Add(itemBlock);
                    }
                }
            }

            return result;
        }
    }
}
