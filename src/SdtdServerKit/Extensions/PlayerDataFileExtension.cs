using System.IO;

namespace SdtdServerKit.Extensions
{
    internal static class PlayerDataFileExtension
    {

        public static int GetInventoryStackCount(this PlayerDataFile pdf, string itemName)
        {
            int count = 0;
            var bag = ProcessInv(pdf.bag, pdf.id);
            var belt = ProcessInv(pdf.inventory, pdf.id);

            foreach (var item in bag)
            {
                if(item.ItemName == itemName)
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

        public static Shared.Models.Inventory GetInventory(this PlayerDataFile pdf)
        {
            try
            {
                return new Shared.Models.Inventory()
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

        public static T ToPlayerDetails<T>(this PlayerDataFile playerDataFile, PersistentPlayerData persistentPlayerData) where T : IPlayerDetails, new()
        {
            var world = GameManager.Instance.World;
            var stats = playerDataFile.ecd.stats;
            var progression = ReadProgressionData(playerDataFile.progressionData);

            return new T()
            {
                PlatformId = persistentPlayerData.NativeId.CombinedString,
                CrossplatformId = persistentPlayerData.PrimaryId.CombinedString,
                PlayerName = persistentPlayerData.PlayerName.DisplayName,
                Position = persistentPlayerData.Position.ToPosition(),
                LastLogin = persistentPlayerData.LastLogin,

                EntityId = playerDataFile.id,
                PlayerKills = playerDataFile.playerKills,
                ZombieKills = playerDataFile.zombieKills,
                Deaths = playerDataFile.deaths,
                Score = playerDataFile.score,
                Health = stats.Health.Value,
                Stamina = stats.Stamina.Value,
                CoreTemp = stats.CoreTemp.Value,
                Food = stats.Food.Value,
                Water = stats.Water.Value,
                Level = progression == null ? 0 : progression.Level,
                ExpToNextLevel = progression == null ? 0 : progression.ExpToNextLevel,
                SkillPoints = progression == null ? 0 : progression.SkillPoints,
                LandProtectionActive = world.IsLandProtectionValidForPlayer(persistentPlayerData),
                LandProtectionMultiplier = world.GetLandProtectionHardnessModifierForPlayer(persistentPlayerData),
                SpawnPoints = playerDataFile.spawnPoints.ToPositions(),
                AlreadyCraftedList = playerDataFile.alreadyCraftedList,
                LastSpawnPosition = playerDataFile.lastSpawnPosition.ToModel(),
                UnlockedRecipeList = playerDataFile.unlockedRecipeList,
                FavoriteRecipeList = playerDataFile.favoriteRecipeList,
                OwnedEntities = playerDataFile.ownedEntities.ToModels(),
                DistanceWalked = playerDataFile.distanceWalked,
                TotalItemsCrafted = playerDataFile.totalItemsCrafted,
                LongestLife = playerDataFile.longestLife,
                CurrentLife = playerDataFile.currentLife,
                TotalTimePlayed = playerDataFile.totalTimePlayed,
                GameStageBornAtWorldTime = playerDataFile.gameStageBornAtWorldTime,
                RentedVMPosition = playerDataFile.rentedVMPosition.ToPosition(),
                RentalEndTime = playerDataFile.rentalEndTime,
                RentalEndDay = playerDataFile.rentalEndDay,
            };
        }

        private static Progression? ReadProgressionData(MemoryStream stream)
        {
            if (stream.Length > 0L)
            {
                using var binaryReader = MemoryPools.poolBinaryReader.AllocSync(false);
                stream.Position = 0L;
                binaryReader.SetBaseStream(stream);

                var progression = new Progression();
                byte b = binaryReader.ReadByte();
                progression.Level = (int)binaryReader.ReadUInt16();
                progression.ExpToNextLevel = binaryReader.ReadInt32();
                progression.SkillPoints = (int)binaryReader.ReadUInt16();
                return progression;
            }

            return null;
        }
    }
}