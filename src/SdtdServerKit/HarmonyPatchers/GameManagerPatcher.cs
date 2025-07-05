using HarmonyLib;
using Noemax.GZip;
using SdtdServerKit.Managers;
using static LightingAround;

namespace SdtdServerKit.HarmonyPatchers
{
    [HarmonyPatch(typeof(GameManager))]
    internal class GameManagerPatcher
    {
        public static bool Before_RequestToSpawnPlayer(ClientInfo _cInfo, int _chunkViewDim, PlayerProfile _playerProfile)
        {
            var xmlsToLoad = WorldStaticData.xmlsToLoad;

            foreach (var item in xmlsToLoad)
            {
                if (item.SendToClients && item.CompressedXmlData != null)
                {
                    var xmlName = item.XmlName;
                    using var compressedMemoryStream = new MemoryStream();
                    using (var deflateOutputStream = new DeflateOutputStream(compressedMemoryStream, 1))
                    {
                        deflateOutputStream.WriteByte(0);
                    }

                    _cInfo.SendPackage(NetPackageManager.GetPackage<NetPackageConfigFile>().Setup(xmlName, compressedMemoryStream.ToArray()));
                }
            }

            return true;
        }

        public static bool Before_ChangeBlocks(GameManager __instance, PlatformUserIdentifierAbs persistentPlayerId, List<BlockChangeInfo> _blocksToChange)
        {
            if (persistentPlayerId == null)
            {
                return true;
            }

            var clientInfo = ConnectionManager.Instance.Clients.ForUserId(persistentPlayerId);

            if (clientInfo == null)
            {
                return true;
            }

            World world = __instance.World;
            foreach (var info in _blocksToChange)
            {
                var blockToChange = info.blockValue.Block;
                if (blockToChange is BlockLandClaim || blockToChange is BlockSleepingBag)
                {
                    var blockPosition = info.pos;
                    int num = GameStats.GetInt(EnumGameStats.ScoreZombieKillMultiplier) >> 2;
                    int num2 = blockPosition.x - num;
                    int num3 = blockPosition.x + num;
                    int num4 = blockPosition.z - num;
                    int num5 = blockPosition.z + num;
                    var poiBulidings = new List<PrefabInstance>();
                    GameManager.Instance.World.GetPOIsAtXZ(num2, num3, num4, num5, poiBulidings);
                    if (poiBulidings.Count > 0)
                    {
                        BlockValue oldBlockValue = world.GetBlock(blockPosition);

                        if(oldBlockValue.type != BlockValue.Air.type)
                        {
                            continue;
                        }

                        info.blockValue = oldBlockValue;

                        NetPackageSetBlock package = NetPackageManager.GetPackage<NetPackageSetBlock>().Setup(null, new List<BlockChangeInfo>() { info }, -1);
                        clientInfo.SendPackage(package);

                        string? message = ConfigManager.GlobalSettings.RemoveSleepingBagFromPoiTip;
                        if(string.IsNullOrEmpty(message) == false)
                        {
                            Utilities.Utils.SendPrivateMessage(new PrivateMessage()
                            {
                                Message = message!,
                                TargetPlayerIdOrName = clientInfo.entityId.ToString(),
                            });
                        }

                        CustomLogger.Info("Land claim detected on: {0}, {1}", blockPosition.x, blockPosition.z);
                    }
                }
            }

            return true;
        }

        public static bool Before_ChangeBlocks_LandClaimProtection(GameManager __instance, PlatformUserIdentifierAbs persistentPlayerId, List<BlockChangeInfo> _blocksToChange)
        {
            if (persistentPlayerId == null)
            {
                return true;
            }

            var clientInfo = ConnectionManager.Instance.Clients.ForUserId(persistentPlayerId);

            if (clientInfo == null)
            {
                return true;
            }

            var playerData = GameManager.Instance.GetPersistentPlayerList().GetPlayerData(persistentPlayerId);
            if (playerData == null)
            {
                return true;
            }

            string? message = null;
            World world = __instance.World;

            List<BlockChangeInfo>? returned = null;

            foreach (var info in _blocksToChange.ToList())
            {
                if (ConfigManager.GlobalSettings.EnableTraderAreaProtection)
                {
                    if (world.IsWithinTraderArea(info.pos))
                    {
                        if (info.blockValue.damage != 0 || info.blockValue.Equals(world.GetBlock(info.pos)) == false)
                        {
                            message = ConfigManager.GlobalSettings.TraderAreaProtectionTip;
                            goto B;
                        }
                        else
                        {
                            continue;
                        }
                    }
                }

                if (ConfigManager.GlobalSettings.EnableLandClaimProtection)
                {
                    if (GameManager.Instance.World.CanPlaceBlockAt(info.pos, playerData, true) == false)
                    {
                        message = ConfigManager.GlobalSettings.LandClaimProtectionTip;
                        goto B;
                    }
                    else
                    {
                        continue;
                    }
                }

                B:
                // returned ??= new List<BlockChangeInfo>();
                // info.blockValue = world.GetBlock(info.pos);
                // returned.Add(info);

                returned ??= new List<BlockChangeInfo>();
                returned.Add(new BlockChangeInfo(info.clrIdx, info.pos, world.GetBlock(info.pos)));

                _blocksToChange.Remove(info);
            }

            if(returned != null && returned.Count > 0)
            {
                NetPackageSetBlock package = NetPackageManager.GetPackage<NetPackageSetBlock>().Setup(null, returned, -1);
                clientInfo.SendPackage(package);
            }
            
            if (string.IsNullOrEmpty(message) == false)
            {
                Utilities.Utils.SendPrivateMessage(new PrivateMessage()
                {
                    Message = message!,
                    TargetPlayerIdOrName = clientInfo.entityId.ToString(),
                });
            }

            return true;
        }

        public static void After_Explosion_AttackBlocks(Explosion __instance)
        {
            List<BlockChangeInfo>? returned = null;
            string? message = null;
            PersistentPlayerData playerDataFromEntityID = GameManager.Instance.persistentPlayers.GetPlayerDataFromEntityID(__instance.entityId);
            foreach (var pos in __instance.ChangedBlockPositions.Keys.ToList())
            {
                if (GameManager.Instance.World.CanPlaceBlockAt(pos, playerDataFromEntityID, true) == false)
                {
                    returned ??= new List<BlockChangeInfo>();
                    returned.Add(new BlockChangeInfo(__instance.clrIdx, pos, GameManager.Instance.World.GetBlock(pos)));

                    __instance.ChangedBlockPositions.Remove(pos);
                    message = ConfigManager.GlobalSettings.LandClaimProtectionTip;
                }
            }

            if (returned != null && returned.Count > 0) 
            {
                NetPackageSetBlock package = NetPackageManager.GetPackage<NetPackageSetBlock>().Setup(null, returned, -1);
                ConnectionManager.Instance.Clients.ForEntityId(__instance.entityId).SendPackage(package);
            }

            if (string.IsNullOrEmpty(message) == false)
            {
                Utilities.Utils.SendPrivateMessage(new PrivateMessage()
                {
                    Message = message!,
                    TargetPlayerIdOrName = __instance.entityId.ToString(),
                });
            }
        }

        //public static bool Before_ChatMessageServer(ClientInfo _cInfo, EChatType _chatType, int _senderEntityId, ref string _msg, List<int> _recipientEntityIds, ref EMessageSender _msgSender)
        //{
        //    if(_msgSender == EMessageSender.SenderIdAsPlayer && LivePlayerManager.TryGetByEntityId(_senderEntityId, out var managedPlayer))
        //    {
        //        var repository = ModApi.ServiceContainer.Resolve<IColoredChatRepository>();
        //        var coloredChat = repository.GetById(managedPlayer!.PlayerId);
        //        if (coloredChat != null)
        //        {
        //            string playerName = managedPlayer.PlayerName;

        //            _msg = $"[{coloredChat.NameColor}]{playerName}[{GetDefaultColor(_chatType)}]: [{coloredChat.TextColor}]{_msg}";
        //            _msgSender = EMessageSender.None;
        //        }
        //    }

        //    return true;
        //}

        //private static string GetDefaultColor(EChatType eChatType)
        //{
        //    switch (eChatType)
        //    {
        //        case EChatType.Global:
        //            return "FFFFFF";
        //        case EChatType.Friends:
        //            return "00BB00";
        //        case EChatType.Party:
        //            return "FFCC00";
        //        case EChatType.Whisper:
        //            return "D00000";
        //        default:
        //            throw new ArgumentOutOfRangeException(nameof(eChatType), eChatType, null);
        //    }
        //}
    }
}