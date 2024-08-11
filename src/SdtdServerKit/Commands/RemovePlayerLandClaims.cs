using SdtdServerKit.Hooks;

namespace SdtdServerKit.Commands
{
    public class RemovePlayerLandClaims : ConsoleCmdBase
    {
        public override string getDescription()
        {
            return "Removes a player's land claims.";
        }

        public override string getHelp()
        {
            return "Removes land claims with the specified id from a oplayer or x y z position.\n" +
                "Usage: ty-rplc {EntityId/PlayerId/PlayerName} [x y z]\n";
        }

        public override string[] getCommands()
        {
            return new string[] { "ty-RemovePlayerLandClaims", "ty-rplc" };
        }

        public override void Execute(List<string> args, CommandSenderInfo _senderInfo)
        {
            try
            {
                var persistentPlayerList = GameManager.Instance.GetPersistentPlayerList();
                if (args.Count == 1)
                {
                    var userId = PlatformUserIdentifierAbs.FromCombinedString(args[0]);
                    if (persistentPlayerList.Players.TryGetValue(userId, out var persistentPlayerData))
                    {
                        var blockChangeInfos = new List<BlockChangeInfo>();
                        foreach (var pos in persistentPlayerData.GetLandProtectionBlocks().ToArray())
                        {
                            persistentPlayerList.RemoveLandProtectionBlock(pos);
                            blockChangeInfos.Add(new BlockChangeInfo(pos, BlockValue.Air, true, false));
                        }
                        
                        GameManager.Instance.SetBlocksRPC(blockChangeInfos, null);

                        var entity = GameManager.Instance.World.GetEntity(persistentPlayerData.EntityId);
                        if (entity != null)
                        {
                            entity.PlayOneShot("keystone_destroyed", false, false, false);
                        }
                        GameManager.Instance.SavePersistentPlayerData();
                    }
                }
                else if (args.Count == 3)
                {
                    int x = int.Parse(args[0]);
                    int y = int.Parse(args[1]);
                    int z = int.Parse(args[2]);

                    var pos = new Vector3i(x, y, z);
                    var persistentPlayerData = persistentPlayerList.GetLandProtectionBlockOwner(pos);
                    if (persistentPlayerData != null)
                    {
                        persistentPlayerList.RemoveLandProtectionBlock(pos);

                        var blockChangeInfos = new List<BlockChangeInfo>() { new BlockChangeInfo(pos, BlockValue.Air, true, false) };
                        GameManager.Instance.SetBlocksRPC(blockChangeInfos, null);

                        var entity = GameManager.Instance.World.GetEntity(persistentPlayerData.EntityId);
                        if (entity != null)
                        {
                            entity.PlayOneShot("keystone_destroyed", false, false, false);
                        }

                        GameManager.Instance.SavePersistentPlayerData();
                    }
                }
                else
                {
                    Log("Wrong number of arguments.");
                    return;
                }
            }
            catch (Exception ex)
            {
                CustomLogger.Error(ex, "Error in RemovePlayerLandClaims.Execute");
            }
        }
    }
}