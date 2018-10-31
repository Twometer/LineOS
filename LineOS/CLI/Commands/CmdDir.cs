using Cosmos.System.FileSystem.Listing;
using System;

namespace LineOS.CLI.Commands
{
    public class CmdDir : ICommand
    {
        public string Name => "dir";

        public string Description => "Lists contents of current directory";

        public string Syntax => "dir <-l>";

        public bool Execute(string[] args)
        {
            var listing = Kernel.FileSystem.GetDirectoryListing(Kernel.ConsoleManager.CurrentPath);
            var extended = args.Length == 1 && args[0] == "-l";
            if (!extended)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                foreach (var f in listing)
                    if (f.mEntryType == DirectoryEntryTypeEnum.Directory)
                        Console.Write(f.mName + "\t");
                Console.ResetColor();
                foreach (var f in listing)
                    if (f.mEntryType == DirectoryEntryTypeEnum.File)
                        Console.Write(f.mName + "\t");
                Console.ResetColor();
                Console.WriteLine();
            }
            else
            {
                Kernel.TablePrinter.WriteHeaders("Type", "Size      ", "Name");
                foreach (var f in listing)
                {
                    var type = f.mEntryType == DirectoryEntryTypeEnum.Directory ? "<DIR>" : "";
                    Kernel.TablePrinter.WriteRow(type, "", f.mName);
                }
            }
            return true;
        }
    }
}
