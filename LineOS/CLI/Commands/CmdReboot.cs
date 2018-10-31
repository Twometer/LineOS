using Cosmos.System;

namespace LineOS.CLI.Commands
{
    public class CmdReboot : ICommand
    {
        public string Name => "reboot";

        public string Description => "Restarts the computer";

        public string Syntax => "reboot";

        public bool Execute(string[] args)
        {
            Power.Reboot();
            return true;
        }
    }
}
