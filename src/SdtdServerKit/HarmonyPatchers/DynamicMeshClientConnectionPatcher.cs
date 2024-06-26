using HarmonyLib;
using Noemax.GZip;

namespace SdtdServerKit.HarmonyPatchers
{
    [HarmonyPatch(typeof(DynamicMeshClientConnection))]
    internal class DynamicMeshClientConnectionPatcher
    {
        [HarmonyPrefix]
        [HarmonyPatch(nameof(DynamicMeshClientConnection.UpdateItemsToSend), new Type[] { typeof(DynamicMeshClientConnection), typeof(NetPackageDynamicClientArrive) })]
        public static bool UpdateItemsToSend(DynamicMeshClientConnection data, NetPackageDynamicClientArrive package)
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

                    package.Sender.SendPackage(NetPackageManager.GetPackage<NetPackageConfigFile>().Setup(xmlName, compressedMemoryStream.ToArray()));
                }
            }

            return true;
        }
    }
}