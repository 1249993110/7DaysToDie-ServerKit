using System.Runtime;

namespace SdtdServerKit.Commands
{
    public class GarbageCollection : ConsoleCmdBase
    {
        public override string getDescription()
        {
            return "Garbage collection";
        }

        public override string getHelp()
        {
            return "Usage: ty-gc";
        }

        public override string[] getCommands()
        {
            return new string[]
            {
                "ty-GarbageCollection",
                "ty-gc"
            };
        }

        public override void Execute(List<string> args, CommandSenderInfo _senderInfo)
        {
            MemoryPools.Cleanup();
            GCSettings.LargeObjectHeapCompactionMode = GCLargeObjectHeapCompactionMode.CompactOnce;
            GC.Collect();
        }
    }
}