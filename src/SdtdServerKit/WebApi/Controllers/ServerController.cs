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
        public IEnumerable<string> ExecuteConsoleCommand(string command, bool inMainThread = false)
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
                Zombies = zombies,
                Entities = world.Entities.Count,
                OnlinePlayers = onlinePlayers,
                OfflinePlayers = offlinePlayers,
                IsBloodMoon = world.aiDirector.BloodMoonComponent.BloodMoonActive,
                FPS = gameManager.fps.Counter,
                Heap = (float)GC.GetTotalMemory(false) / 1048576F,
                MaxHeap = (float)GameManager.MaxMemoryConsumption / 1048576F,
                Chunks = Chunk.InstanceCount,
                CGO = world.m_ChunkManager.GetDisplayedChunkGameObjectsCount(),
                Items = EntityItem.ItemInstanceCount,
                ChunkObservedEntities = world.m_ChunkManager.m_ObservedEntities.Count,
                ResidentSetSize = (float)GetRSS.GetCurrentRSS() / 1024F / 1024F,
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
                FormatCommandArgs(giveItemEntry.TargetPlayerIdOrName),
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
                FormatCommandArgs(globalMessage.Message),
                FormatCommandArgs(globalMessage.SenderName));

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
                FormatCommandArgs(privateMessage.TargetPlayerIdOrName),
                FormatCommandArgs(privateMessage.Message),
                FormatCommandArgs(privateMessage.SenderName));

            return ExecuteConsoleCommand(cmd);
        }

        /// <summary>
        /// Restart server.
        /// </summary>
        [HttpPost]
        [Route(nameof(RestartServer))]
        public IEnumerable<string> RestartServer([FromUri]bool force = false)
        {
            string cmd = "ty-rs";
            if (force)
            {
                cmd += " -f";
            }

            return ExecuteConsoleCommand(cmd);
        }

        private static string FormatCommandArgs(string? args)
        {
            if (args == null)
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
