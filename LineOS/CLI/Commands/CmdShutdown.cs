using Cosmos.System;

namespace LineOS.CLI.Commands
{
    public class CmdShutdown : ICommand
    {
        public string Name => "shutdown";

        public string Description => "Shuts down LineOS";

        public string Syntax => "shutdown";

        public bool Execute(string[] args)
        {
            Power.Shutdown();
            return true;
        }
    }
}
