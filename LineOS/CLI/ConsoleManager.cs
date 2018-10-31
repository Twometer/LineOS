using System;

namespace LineOS.CLI
{
    public class ConsoleManager
    {
        public string CurrentPath { get; set; } = "0:\\";
        public CommandHandler CommandHandler { get; } = new CommandHandler();

        public void HandleConsole()
        {
            WritePrompt();

            var input = Console.ReadLine();
            if (string.IsNullOrEmpty(input))
                return;

            try
            {
                CommandHandler.HandleCommand(input);
            }
            catch (Exception e)
            {
                WriteError(e);
            }

            Console.WriteLine();
        }

        private void WriteError(Exception e)
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.BackgroundColor = ConsoleColor.Blue;
            Console.WriteLine("Internal system error");
            Console.WriteLine(e.ToString());
            Console.ResetColor();
        }

        private void WritePrompt()
        {
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.Write("$");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write("root");
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.Write("@");
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.Write("lineos");
            Console.ResetColor();
            Console.Write(" " + CurrentPath + "> ");
        }

    }
}
