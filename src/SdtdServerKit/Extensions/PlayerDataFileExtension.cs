namespace SdtdServerKit.Extensions
{
    /// <summary>
    /// Extension methods for PlayerDataFile.
    /// </summary>
    internal static class PlayerDataFileExtension
    {
        /// <summary>
        /// Gets the total stack count of a specific item in the player's inventory.
        /// </summary>
        /// <param name="pdf">The PlayerDataFile instance.</param>
        /// <param name="itemName">The name of the item.</param>
        /// <returns>The total stack count of the item.</returns>
        public static int GetInventoryStackCount(this PlayerDataFile pdf, string itemName)
        {
            int count = 0;
            var bag = ProcessInv(pdf.bag, pdf.id);
            var belt = ProcessInv(pdf.inventory, pdf.id);

            foreach (var item in bag)
            {
                if (item.ItemName == itemName)
                {
                    count += item.Count;
                }
            }

            foreach (var item in belt)
            {
                if (item.ItemName == itemName)
                {
                    count += item.Count;
                }
            }

            return count;
        }

        #region Get Inventory

        /// <summary>
        /// Gets the player's inventory.
        /// </summary>
        /// <param name="pdf">The PlayerDataFile instance.</param>
        /// <returns>The player's inventory.</returns>
        public static Models.Inventory GetInventory(this PlayerDataFile pdf)
        {
            try
            {
                return new Models.Inventory()
                {
                    Bag = ProcessInv(pdf.bag, pdf.id),
                    Belt = ProcessInv(pdf.inventory, pdf.id),
                    Equipment = ProcessEqu(pdf.equipment, pdf.id)
                };
            }
            catch (Exception ex)
            {
                throw new Exception("Get player inventory from PlayerDataFile failed.", ex);
            }
        }

        private static IEnumerable<InvItem> ProcessInv(ItemStack[] sourceFields, int entityId)
        {
            var target = new List<InvItem>(sourceFields.Length);

            foreach (var field in sourceFields)
            {
                var invItem = CreateInvItem(field.itemValue, field.count, entityId);
                if (invItem != null && field.itemValue.Modifications != null)
                {
                    ProcessParts(field.itemValue.Modifications, invItem, entityId);
                    target.Add(invItem);
                }
            }

            return target;
        }

        private static IEnumerable<InvItem> ProcessEqu(Equipment sourceEquipment, int entityId)
        {
            //int slotCount = sourceEquipment.GetSlotCount();
            //var equipment = new InvItem?[slotCount];
            //for (int i = 0; i < slotCount; i++)
            //{
            //    equipment[i] = CreateInvItem(sourceEquipment.GetSlotItem(i), 1, entityId);
            //}

            int slotCount = sourceEquipment.GetSlotCount();
            var target = new List<InvItem>(slotCount);
            for (int i = 0; i < slotCount; i++)
            {
                var invItem = CreateInvItem(sourceEquipment.GetSlotItem(i), 1, entityId);
                if (invItem != null)
                {
                    target.Add(invItem);
                }
            }

            return target;
        }

        private static void ProcessParts(ItemValue[] parts, InvItem item, int entityId)
        {
            int length = parts.Length;

            InvItem?[] itemParts = new InvItem[length];

            for (int i = 0; i < length; i++)
            {
                var partItem = CreateInvItem(parts[i], 1, entityId);
                if (partItem != null && parts[i].Modifications != null)
                {
                    ProcessParts(parts[i].Modifications, partItem, entityId);
                }

                itemParts[i] = partItem;
            }

            item.Parts = itemParts;
        }

        private static InvItem? CreateInvItem(ItemValue itemValue, int count, int entityId)
        {
            try
            {
                if (count <= 0 || itemValue == null || itemValue.Equals(ItemValue.None))
                {
                    return null;
                }

                var itemClass = ItemClass.list[itemValue.type];

                if (itemClass == null)
                {
                    return null;
                }

                //int maxAllowed = itemClass.Stacknumber.Value;
                string name = itemClass.GetItemName();

                //string steamId = ConnectionManager.Instance.Clients.ForEntityId(entityId).playerId;

                //var inventoryCheck = FunctionManager.AntiCheat.InventoryCheck;
                //if (inventoryCheck.IsEnabled)
                //{
                //    inventoryCheck.Execute(steamId, name, count, maxAllowed);
                //}

                int quality = 0;
                string? qualityColor = null;
                if (itemValue.HasQuality)
                {
                    quality = itemValue.Quality;
                    qualityColor = QualityInfo.GetQualityColorHex(quality);
                }
                else
                {
                    quality = -1;
                }

                InvItem item = new InvItem()
                {
                    ItemName = name,
                    Count = count,
                    Quality = quality,
                    IconName = itemClass.GetIconName(),
                    IconColor = itemClass.GetIconTint().ToHex(),
                    QualityColor = qualityColor,
                    MaxUseTimes = itemValue.MaxUseTimes,
                    UseTimes = itemValue.UseTimes
                };

                return item;
            }
            catch (Exception ex)
            {
                CustomLogger.Warn(ex, "Error in PlayerDataFileExtension.CreateInvItem" + Environment.NewLine + JsonConvert.SerializeObject(itemValue));
                return null;
            }
        }

        #endregion Get Inventory
    }
}