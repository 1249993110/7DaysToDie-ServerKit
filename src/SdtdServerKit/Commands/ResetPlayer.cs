using System.Security.Cryptography;
using Webserver.WebAPI.APIs.WorldState;

namespace SdtdServerKit.Commands
{
    public class ResetPlayer : ConsoleCmdBase
    {
        public override string getDescription()
        {
            return "Reset a players profile. * WARNING * Can not be undone without a backup.";
        }

        public override string getHelp()
        {
            return "Usage: ty-rpp <EOS/EntityId/PlayerName>";
        }

        public override string[] getCommands()
        {
            return new string[] { "ty-ResetPlayerProfile", "ty-rpp" };
        }

        public override void Execute(List<string> args, CommandSenderInfo _senderInfo)
        {
            try
            {
                if (args.Count != 1)
                {
                    Log("Wrong number of arguments, expected 1, found '{0}'", args.Count);
                    return;
                }

                var cInfo = ConsoleHelper.ParseParamIdOrName(args[0]);
                if (cInfo != null && GameManager.Instance.World.Players.dict.TryGetValue(cInfo.entityId, out EntityPlayer entityPlayer))
                {
                    SdtdConsole.Instance.ExecuteSync(string.Format("kick {0}", cInfo.CrossplatformId.CombinedString), ModApi.CmdExecuteDelegate);
                    GameManager.Instance.World.aiDirector.RemoveEntity(entityPlayer);
                    ResetProfileExec(cInfo.CrossplatformId);
                    return;
                }

                var userId = PlatformUserIdentifierAbs.FromCombinedString(args[0]);
                if(userId != null)
                {
                    var persistentPlayersDict = GameManager.Instance.GetPersistentPlayerList().Players;
                    if (persistentPlayersDict.TryGetValue(userId, out var persistentPlayerData))
                    {
                        ResetProfileExec(persistentPlayerData.PrimaryId);
                        return;
                    }
                }

                Log("Unable to find player data for id '{0}'", args[0]);
            }
            catch (Exception e)
            {
                Log(string.Format("Error in ResetPlayer.Execute: {0}", e.Message));
            }
        }

        public void ResetProfileExec(PlatformUserIdentifierAbs _id)
        {
            try
            {
                RemovePersistentData(_id);
                RemoveProfile(_id);
            }
            catch (Exception e)
            {
                Log(string.Format("Error in ResetPlayer.ResetProfileExec: {0}", e.Message));
            }
        }

        public static void RemovePersistentData(PlatformUserIdentifierAbs uId)
        {
            var list = GameManager.Instance.GetPersistentPlayerList();
            if (list.Players.ContainsKey(uId))
            {
                list.Players.Remove(uId);
                list.Write(GameIO.GetSaveGameDir(null, null) + "/players.xml");
            }
        }

        public void RemoveProfile(PlatformUserIdentifierAbs uId)
        {
            try
            {
                string filepath = string.Format("{0}/Player/{1}.map", GameIO.GetSaveGameDir(), uId.CombinedString);
                if (!File.Exists(filepath))
                {
                    Log(string.Format("Could not find file '{0}' for player profile reset", filepath));
                }
                else
                {
                    File.Delete(filepath);
                    Log(string.Format("File '{0}' deleted for player profile reset", filepath));
                }
                Log(string.Format("Removing .ttp file in three seconds"));
                Task.Delay(3000).ContinueWith((task) => 
                {
                    DelayedProfileDeletion(uId);
                }).Start();
            }
            catch (Exception e)
            {
                Log(string.Format("Error in ResetPlayer.RemoveProfile: {0}", e.Message));
            }
        }

        public void DelayedProfileDeletion(PlatformUserIdentifierAbs uId)
        {
            try
            {
                string filepath = string.Format("{0}/Player/{1}.ttp", GameIO.GetSaveGameDir(), uId.CombinedString);
                if (!File.Exists(filepath))
                {
                    Log(string.Format("Could not find file '{0}' for player profile reset", filepath));
                }
                else
                {
                    File.Delete(filepath);
                    Log(string.Format("File '{0}' deleted for player profile reset", filepath));
                }
            }
            catch (Exception e)
            {
                Log(string.Format("Error in ResetPlayer.DelayedProfileDeletion: {0}", e.Message));
            }
        }

    }
}

