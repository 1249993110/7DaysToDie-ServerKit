using HarmonyLib;
using Noemax.GZip;
using SdtdServerKit.Data.IRepositories;
using SdtdServerKit.Managers;
using SdtdServerKit.Models;
using System.Runtime.Remoting.Messaging;

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