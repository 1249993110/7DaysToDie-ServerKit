//using HarmonyLib;
//using Noemax.GZip;
//using System.Collections;
//using System.Reflection;
//using System.Xml;

//namespace SdtdServerKit.HarmonyPatchers
//{
//    [HarmonyPatch(typeof(DynamicMeshClientConnection))]
//    internal class DynamicMeshClientConnectionPatcher
//    {
//        [HarmonyPrefix]
//        [HarmonyPatch(nameof(DynamicMeshClientConnection.UpdateItemsToSend), new Type[] { typeof(DynamicMeshClientConnection), typeof(NetPackageDynamicClientArrive) })]
//        public static bool UpdateItemsToSend(DynamicMeshClientConnection data, NetPackageDynamicClientArrive package)
//        {
//            // WorldStaticData.SendXmlsToClient(package.Sender);

//            if (typeof(WorldStaticData).GetField("xmlsToLoad", BindingFlags.Static | BindingFlags.NonPublic).GetValue(null) is IEnumerable xmlsToLoad)
//            {
//                Type? type = null;
//                FieldInfo? nameField = null;
//                FieldInfo? dataField = null;
//                FieldInfo? sendToClientField = null;
//                FieldInfo? loadClientField = null;

//                foreach (var item in xmlsToLoad)
//                {
//                    if (type == null || dataField == null || nameField == null || sendToClientField == null || loadClientField == null)
//                    {
//                        type = item.GetType();
//                        nameField = type.GetField("XmlName", BindingFlags.Instance | BindingFlags.Public);
//                        dataField = type.GetField("CompressedXmlData", BindingFlags.Instance | BindingFlags.Public);
//                        sendToClientField = type.GetField("SendToClients", BindingFlags.Instance | BindingFlags.Public);
//                        loadClientField = type.GetField("LoadClientFile", BindingFlags.Instance | BindingFlags.Public);
//                    }

//                    if ((bool)sendToClientField.GetValue(item)
//                        && (dataField.GetValue(item) is byte[] xmlData && xmlData != null))
//                    {
//                        var xmlName = (string)nameField.GetValue(item);
//                        using var compressedMemoryStream = new MemoryStream();
//                        using (var deflateOutputStream = new DeflateOutputStream(compressedMemoryStream, 1))
//                        {
//                            deflateOutputStream.WriteByte(0);
//                        }

//                        package.Sender.SendPackage(NetPackageManager.GetPackage<NetPackageConfigFile>().Setup(xmlName, compressedMemoryStream.ToArray()));
//                    }
//                }
//            }

//            return true;
//        }
//    }
//}