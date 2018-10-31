using LineOS.CLI.Commands;
using System;

namespace LineOS.CLI
{
    public class CommandHandler
    {
        private static readonly string[] EmptyArray = new string[0];

        public ICommand[] Commands { get; } = {
            new CmdShutdown(),
            new CmdReboot(),
            new CmdDir(),
            new CmdChgDir(),
            new CmdVolumeList(),
            new CmdKbdLayout(),
            new CmdHelp(),
            new CmdGui(),
            new CmdReadFile(),
            new CmdClearScreen()
        };

        public void HandleCommand(string command)
        {
            var arr = command.Trim().Split(' ');
            var name = arr[0];
            var args = arr.Length > 1 ? new string[arr.Length - 1] : EmptyArray;
            if (args.Length > 0)
                Array.Copy(arr, 1, args, 0, args.Length);
            foreach (var cmd in Commands)
                if (cmd.Name.ToLower() == name)
                {
                    if (!cmd.Execute(args))
                        Console.WriteLine("Syntax: " + cmd.Syntax);
                    return;
                }
            Console.WriteLine("SYNTAX ERROR");
        }

    }
}
