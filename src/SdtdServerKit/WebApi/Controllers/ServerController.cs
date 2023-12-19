using Newtonsoft.Json.Linq;
using SystemInformation;
using UnityEngine;

namespace SdtdServerKit.WebApi.Controllers
{
    /// <summary>
    /// Server
    /// </summary>
    [Authorize]
    [RoutePrefix("api/Server")]
    public class ServerController : ApiController
    {
        /// <summary>
        /// Execute console command.
        /// </summary>
        /// <param name="command">Command</param>
        /// <param name="inMainThread">Execute console command in main thread</param>
        /// <returns></returns>
        [HttpPost]
        [Route(nameof(ExecuteConsoleCommand))]
        public IEnumerable<string> ExecuteConsoleCommand([FromUri] string command, [FromUri] bool inMainThread = false)
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
        /// Get allowed commands.
        /// </summary>
        [HttpGet]
        [Route(nameof(AllowedCommands))]
        public IEnumerable<AllowedCommand> AllowedCommands()
        {
            var allowedCommands = new List<AllowedCommand>();
            foreach (var consoleCommand in SdtdConsole.Instance.GetCommands())
            {
                var commands = consoleCommand.GetCommands();
                int commandPermissionLevel = GameManager.Instance.adminTools.Commands.GetCommandPermissionLevel(commands);

                allowedCommands.Add(new AllowedCommand()
                {
                    Commands = commands,
                    PermissionLevel = commandPermissionLevel,
                    Description = consoleCommand.GetDescription(),
                    Help = consoleCommand.GetHelp(),
                });
            }

            return allowedCommands;
        }

        /// <summary>
        /// Get allowed commands.
        /// </summary>
        [HttpGet]
        [Route(nameof(AllowSpawnedEntities))]
        public IEnumerable<AllowSpawnedEntity> AllowSpawnedEntities()
        {
            var result = new List<AllowSpawnedEntity>();
            int num = 1;
            foreach (var item in EntityClass.list.Dict.Values)
            {
                if (item.userSpawnType == EntityClass.UserSpawnType.Menu)
                {
                    result.Add(new AllowSpawnedEntity()
                    {
                        Id = num,
                        Name = item.entityClassName
                    });

                    ++num;
                }
            }

            return result;
        }

        /// <summary>
        /// Get stats.
        /// </summary>
        [HttpGet]
        [Route(nameof(Stats))]
        public Stats Stats()
        {
            var gameManager = GameManager.Instance;
            var world = GameManager.Instance.World;
            var worldTime = world.GetWorldTime();
            var entityList = world.Entities.list;

            int zombies = 0;
            int animals = 0;
            foreach (var entity in entityList)
            {
                if (entity.IsAlive())
                {
                    if (entity is EntityEnemy)
                    {
                        zombies++;
                    }
                    else if (entity is EntityAnimal)
                    {
                        animals++;
                    }
                }
            }

            int onlinePlayers = world.Players.Count;
            int offlinePlayers = gameManager.GetPersistentPlayerList().Players.Count - onlinePlayers;
            return new Stats()
            {
                Uptime = Time.timeSinceLevelLoad,
                GameTime = new GameTime()
                {
                    Days = GameUtils.WorldTimeToDays(worldTime),
                    Hours = GameUtils.WorldTimeToHours(worldTime),
                    Minutes = GameUtils.WorldTimeToMinutes(worldTime),
                },
                Animals = animals,
                MaxAnimals = GamePrefs.GetInt(EnumGamePrefs.MaxSpawnedAnimals),
                Zombies = zombies,
                MaxZombies = GamePrefs.GetInt(EnumGamePrefs.MaxSpawnedZombies),
                Entities = world.Entities.Count,
                OnlinePlayers = onlinePlayers,
                MaxOnlinePlayers = GamePrefs.GetInt(EnumGamePrefs.ServerMaxPlayerCount),
                OfflinePlayers = offlinePlayers,
                IsBloodMoon = world.aiDirector.BloodMoonComponent.BloodMoonActive,
                FPS = gameManager.fps.Counter,
                Heap = (float)GC.GetTotalMemory(false) / 1048576F,
                MaxHeap = (float)GameManager.MaxMemoryConsumption / 1048576F,
                Chunks = Chunk.InstanceCount,
                CGO = world.m_ChunkManager.GetDisplayedChunkGameObjectsCount(),
                Items = EntityItem.ItemInstanceCount,
                ChunkObservedEntities = world.m_ChunkManager.m_ObservedEntities.Count,
                ResidentSetSize = (float)GetRSS.GetCurrentRSS() / 1048576F,
                ServerVersion = Constants.cVersionInformation.LongString,
                ServerIp = GamePrefs.GetString(EnumGamePrefs.ServerIP),
                ServerPort = GamePrefs.GetInt(EnumGamePrefs.ServerPort),
                GameMode = GamePrefs.GetString(EnumGamePrefs.GameMode),
                GameWorld = GamePrefs.GetString(EnumGamePrefs.GameWorld),
                GameName = GamePrefs.GetString(EnumGamePrefs.GameName),
                GameDifficulty = GamePrefs.GetInt(EnumGamePrefs.GameDifficulty),
            };
        }

        /// <summary>
        ///系统信息
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route(nameof(SystemInfo))]
        public Shared.Models.SystemInfo SystemInfo()
        {
            return new Shared.Models.SystemInfo()
            {
                DeviceModel = UnityEngine.Device.SystemInfo.deviceModel,
                DeviceName = UnityEngine.Device.SystemInfo.deviceName,
                DeviceType = UnityEngine.Device.SystemInfo.deviceType.ToString(),
                DeviceUniqueIdentifier = UnityEngine.Device.SystemInfo.deviceUniqueIdentifier,
                OperatingSystem = UnityEngine.Device.SystemInfo.operatingSystem,
                OperatingSystemFamily = UnityEngine.Device.SystemInfo.operatingSystemFamily.ToString(),
                ProcessorCount = UnityEngine.Device.SystemInfo.processorCount,
                ProcessorFrequency = UnityEngine.Device.SystemInfo.processorFrequency,
                ProcessorType = UnityEngine.Device.SystemInfo.processorType,
                SystemMemorySize = UnityEngine.Device.SystemInfo.systemMemorySize,
            };
        }

        /// <summary>
        /// Get game infomation.
        /// </summary>
        [HttpGet]
        [Route(nameof(GameInfo))]
        public JObject GameInfo()
        {
            var gsi = ConnectionManager.Instance.LocalServerInfo;
            var serverInfo = new JObject();

            foreach (string stringGamePref in Enum.GetNames(typeof(GameInfoString)))
            {
                string value = gsi.GetValue((GameInfoString)Enum.Parse(typeof(GameInfoString), stringGamePref));

                var singleStat = new JObject()
                {
                    { "type", "string" },
                    { "value", value }
                };

                serverInfo.Add(stringGamePref, singleStat);
            }

            foreach (string intGamePref in Enum.GetNames(typeof(GameInfoInt)))
            {
                int value = gsi.GetValue((GameInfoInt)Enum.Parse(typeof(GameInfoInt), intGamePref));

                var singleStat = new JObject()
                {
                    { "type", "int" },
                    { "value", value }
                };

                serverInfo.Add(intGamePref, singleStat);
            }

            foreach (string boolGamePref in Enum.GetNames(typeof(GameInfoBool)))
            {
                bool value = gsi.GetValue((GameInfoBool)Enum.Parse(typeof(GameInfoBool), boolGamePref));

                var singleStat = new JObject()
                {
                    { "type", "bool" },
                    { "value", value }
                };

                serverInfo.Add(boolGamePref, singleStat);
            }

            return serverInfo;
        }

        /// <summary>
        /// Get game infomation.
        /// </summary>
        [HttpPost]
        [Route(nameof(GiveItem))]
        public IEnumerable<string> GiveItem([FromBody]GiveItem giveItemEntry)
        {
            string cmd = string.Format("ty-gi {0} {1} {2} {3} {4}",
                Utils.FormatCommandArgs(giveItemEntry.TargetPlayerIdOrName),
                giveItemEntry.ItemName,
                giveItemEntry.Count,
                giveItemEntry.Quality,
                giveItemEntry.Durability);

            return ExecuteConsoleCommand(cmd);
        }

        /// <summary>
        /// Send global message.
        /// </summary>
        [HttpPost]
        [Route(nameof(SendGlobalMessage))]
        public IEnumerable<string> SendGlobalMessage([FromBody] GlobalMessage globalMessage)
        {
            string cmd = string.Format("ty-say {0} {1}",
                Utils.FormatCommandArgs(globalMessage.Message),
                Utils.FormatCommandArgs(globalMessage.SenderName));

            return ExecuteConsoleCommand(cmd);
        }

        /// <summary>
        /// Send private message.
        /// </summary>
        [HttpPost]
        [Route(nameof(SendPrivateMessage))]
        public IEnumerable<string> SendPrivateMessage([FromBody] PrivateMessage privateMessage)
        {
            string cmd = string.Format("ty-pm {0} {1} {2}",
                Utils.FormatCommandArgs(privateMessage.TargetPlayerIdOrName),
                Utils.FormatCommandArgs(privateMessage.Message),
                Utils.FormatCommandArgs(privateMessage.SenderName));

            return ExecuteConsoleCommand(cmd);
        }

        /// <summary>
        /// Restart server.
        /// </summary>
        [HttpPost]
        [Route(nameof(Restart))]
        public IEnumerable<string> Restart([FromUri]bool force = false)
        {
            string cmd = "ty-rs";
            if (force)
            {
                cmd += " -f";
            }

            return ExecuteConsoleCommand(cmd);
        }

        /// <summary>
        /// Shutdown server.
        /// </summary>
        [HttpDelete]
        [Route(nameof(Shutdown))]
        public IEnumerable<string> Shutdown()
        {
            return ExecuteConsoleCommand("shutdown");
        }

        /// <summary>
        /// Reset Player.
        /// </summary>
        [HttpDelete]
        [Route(nameof(ResetPlayer))]
        public IEnumerable<string> ResetPlayer([FromUri] string playerId)
        {
            return ExecuteConsoleCommand("ty-rpp " + playerId);
        }

    }
}
