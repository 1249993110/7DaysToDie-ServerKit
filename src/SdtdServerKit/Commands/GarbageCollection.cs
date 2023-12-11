using System.Runtime;

namespace SdtdServerKit.Commands
{
    public class GarbageCollection : ConsoleCmdBase
    {
        protected override string getDescription()
        {
            return "Garbage collection";
        }

        protected override string getHelp()
        {
            return "Usage: ty-gc";
        }

        protected override string[] getCommands()
        {
            return new string[]
            {
                "ty-GarbageCollection",
                "ty-gc"
            };
        }

        public override void Execute(List<string> args, CommandSenderInfo _senderInfo)
        {
            GCSettings.LargeObjectHeapCompactionMode = GCLargeObjectHeapCompactionMode.CompactOnce;
            GC.Collect();
        }
    }
}