namespace SdtdServerKit
{
    /// <summary>
    /// 工具类
    /// </summary>
    public static class Utils
    {
        /// <summary>
        /// 获取玩家背包物品数量
        /// </summary>
        /// <param name="playerId"></param>
        /// <param name="itemName"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public static int GetPlayerInventoryStackCount(string playerId, string itemName)
        {
            var userId = PlatformUserIdentifierAbs.FromCombinedString(playerId);
            if(userId == null)
            {
                throw new Exception("Invalid player id.");
            }

            var clientInfo = ConnectionManager.Instance.Clients.ForUserId(userId);
            if (clientInfo == null)
            {
                throw new Exception("Player not found.");
            }

            return clientInfo.latestPlayerData.GetInventoryStackCount(itemName);
        }

        /// <summary>
        /// 给予玩家物品
        /// </summary>
        /// <param name="playerIdOrName"></param>
        /// <param name="itemName"></param>
        /// <param name="count"></param>
        /// <param name="quality"></param>
        /// <param name="durability"></param>
        public static void GiveItem(string playerIdOrName, string itemName, int count, int quality = 0, int durability = 0)
        {
            ExecuteConsoleCommand($"ty-gi {FormatCommandArgs(playerIdOrName)} {itemName} {count} {quality} {durability}");
        }

        /// <summary>
        /// 检查玩家周围是否有僵尸
        /// </summary>
        /// <param name="_player"></param>
        /// <returns></returns>
        public static bool ZombieCheck(EntityPlayer _player)
        {
            var entities = GameManager.Instance.World.Entities.list;
            for (int i = 0; i < entities.Count; i++)
            {
                Entity entity = entities[i];
                if (entity is EntityZombie zombie && zombie.IsSpawned() && zombie.IsAlive())
                {
                    if ((_player.serverPos.ToVector3() / 32F - entity.position).sqrMagnitude <= 20 * 20)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// 判断两个玩家是否是好友
        /// </summary>
        /// <param name="entityId"></param>
        /// <param name="anotherEntityId"></param>
        /// <returns></returns>
        public static bool IsFriend(int entityId, int anotherEntityId)
        {
            var players = GameManager.Instance.World.Players.dict;
            if (players.TryGetValue(entityId, out var entityPlayer) == false)
            {
                return false;
            }

            if (players.TryGetValue(anotherEntityId, out var anotherEntityPlayer) == false)
            {
                return false;
            }

            return entityPlayer.IsFriendsWith(anotherEntityPlayer);
        }

        /// <summary>
        /// 获取玩家位置
        /// </summary>
        /// <param name="entityId"></param>
        /// <returns></returns>
        public static Position GetPlayerPosition(int entityId)
        {
            if(GameManager.Instance.World.Players.dict.TryGetValue(entityId, out EntityPlayer player))
            {
                return player.position.ToPosition();
            }

            var clientInfo = ConnectionManager.Instance.Clients.ForEntityId(entityId);
            if (clientInfo != null)
            {
                if(GameManager.Instance.GetPersistentPlayerList().Players.TryGetValue(clientInfo.InternalId, out PersistentPlayerData persistentPlayerData))
                {
                    return persistentPlayerData.Position.ToPosition();
                }
            }

            throw new Exception("Player not found.");
        }

        /// <summary>
        /// 传送玩家
        /// </summary>
        /// <param name="originPlayerIdOrName"></param>
        /// <param name="targetPlayerIdOrNameOrPosition"></param>
        /// <returns></returns>
        public static IEnumerable<string> TeleportPlayer(string originPlayerIdOrName, string targetPlayerIdOrNameOrPosition)
        {
            string target = targetPlayerIdOrNameOrPosition;
            if (target.Split(' ').Length != 3)
            {
                target = FormatCommandArgs(target);
            }

            return ExecuteConsoleCommand($"tele {FormatCommandArgs(originPlayerIdOrName)} {target}");
        }

        /// <summary>
        /// 发送全局消息
        /// </summary>
        /// <param name="globalMessage"></param>
        /// <returns></returns>
        public static IEnumerable<string> SendGlobalMessage(GlobalMessage globalMessage)
        {
            string cmd = string.Format("ty-say {0} {1}", 
                FormatCommandArgs(globalMessage.Message), 
                FormatCommandArgs(globalMessage.SenderName));
            return ExecuteConsoleCommand(cmd);
        }

        /// <summary>
        /// 发送私信
        /// </summary>
        /// <param name="privateMessage"></param>
        /// <returns></returns>
        public static IEnumerable<string> SendPrivateMessage(PrivateMessage privateMessage)
        {
            string cmd = string.Format("ty-pm {0} {1} {2}",
                FormatCommandArgs(privateMessage.TargetPlayerIdOrName),
                FormatCommandArgs(privateMessage.Message),
                FormatCommandArgs(privateMessage.SenderName));

            return ExecuteConsoleCommand(cmd);
        }

        /// <summary>
        /// 执行控制台命令
        /// </summary>
        /// <param name="command"></param>
        /// <param name="inMainThread"></param>
        /// <returns></returns>
        public static IEnumerable<string> ExecuteConsoleCommand(string command, bool inMainThread = false)
        {
            if (inMainThread)
            {
                IEnumerable<string> executeResult = Enumerable.Empty<string>();
                ModApi.MainThreadSyncContext.Send((state) =>
                {
                    executeResult = SdtdConsole.Instance.ExecuteSync((string)state, ModApi.CmdExecuteDelegate);
                }, command);

                return executeResult;
            }
            else
            {
                return SdtdConsole.Instance.ExecuteSync(command, ModApi.CmdExecuteDelegate);
            }
        }

        /// <summary>
        /// 获取血月剩余天数
        /// </summary>
        /// <param name="daysUntilHorde"></param>
        /// <returns></returns>
        public static int DaysRemaining(int daysUntilHorde)
        {
            int bloodmoonFrequency = GamePrefs.GetInt(EnumGamePrefs.BloodMoonFrequency);
            if (daysUntilHorde <= bloodmoonFrequency)
            {
                int daysLeft = bloodmoonFrequency - daysUntilHorde;
                return daysLeft;
            }
            else
            {
                int daysLeft = daysUntilHorde - bloodmoonFrequency;
                return DaysRemaining(daysLeft);
            }
        }

        /// <summary>
        /// 格式化命令参数
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public static string FormatCommandArgs(string args)
        {
            if (string.IsNullOrEmpty(args))
            {
                return string.Empty;
            }

            if (args[0] == '\"' && args[args.Length - 1] == '\"')
            {
                return args;
            }

            if (args.Contains('\"'))
            {
                throw new Exception("Parameters should not contain the character double quotes.");
            }

            if (args.Contains(' '))
            {
                return string.Concat("\"", args, "\"");
            }

            return args;
        }
    }
}