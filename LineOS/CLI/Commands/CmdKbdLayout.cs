using Cosmos.System;
using Cosmos.System.ScanMaps;

namespace LineOS.CLI.Commands
{
    public class CmdKbdLayout : ICommand
    {
        public string Name => "kbd";

        public string Description => "Switch keyboard layout";

        public string Syntax => "kbd <us|de|fr>";

        public bool Execute(string[] args)
        {
            if (args.Length != 1)
                return false;
            var layout = args[0].ToLower();
            switch (layout)
            {
                case "us":
                    KeyboardManager.SetKeyLayout(new US_Standard());
                    break;
                case "de":
                    KeyboardManager.SetKeyLayout(new DE_Standard());
                    break;
                case "fr":
                    KeyboardManager.SetKeyLayout(new FR_Standard());
                    break;
                default:
                    System.Console.WriteLine("Unknown keyboard layout " + layout);
                    break;
            }
            return true;
        }
    }
}
