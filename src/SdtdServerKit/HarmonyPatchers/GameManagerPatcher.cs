using HarmonyLib;
using Noemax.GZip;

namespace SdtdServerKit.HarmonyPatchers
{
    [HarmonyPatch(typeof(GameManager))]
    internal class GameManagerPatcher
    {
        [HarmonyPrefix]
        [HarmonyPatch(nameof(GameManager.RequestToSpawnPlayer))]
        public static bool RequestToSpawnPlayer(ClientInfo _cInfo, int _chunkViewDim, PlayerProfile _playerProfile)
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
    }
}