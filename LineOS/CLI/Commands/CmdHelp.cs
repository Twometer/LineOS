namespace LineOS.CLI.Commands
{
    public class CmdHelp : ICommand
    {
        public string Name => "help";

        public string Description => "Show a list of available commands";

        public string Syntax => "help";

        public bool Execute(string[] args)
        {
            Kernel.TablePrinter.WriteHeaders("Command", "Description");
            foreach(var cmd in Kernel.ConsoleManager.CommandHandler.Commands)
                Kernel.TablePrinter.WriteRow(cmd.Name, cmd.Description);
            return true;
        }
    }
}
