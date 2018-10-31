using System;
using Cosmos.System.FileSystem.VFS;
using LineOS.CLI;
using LineOS.FS;
using Sys = Cosmos.System;

namespace LineOS
{
    public class Kernel : Sys.Kernel
    {
        public static VFSBase FileSystem { get; private set; }
        public static ConsoleManager ConsoleManager { get; private set; }
        public static TablePrinter TablePrinter { get; private set; }

        protected override void BeforeRun()
        {
            FileSystem = new NtfsAwareCosmosVfs();
            ConsoleManager = new ConsoleManager();
            TablePrinter = new TablePrinter();

            VFSManager.RegisterVFS(FileSystem);
            Console.WriteLine("Startup completed");
            Console.Clear();

            Console.WriteLine("LineOS 1.0.0");
            Console.WriteLine("(c) 2018 Twometer Applications");
            Console.WriteLine();
        }

        protected override void Run()
        {
            ConsoleManager.HandleConsole();
            /*Console.ForegroundColor = ConsoleColor.Blue;
            Console.Write("$");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write("root");
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.Write("@");
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.Write("lineos");
            Console.ResetColor();
            Console.Write(" " + currentPath + "> ");

            var input = Console.ReadLine();
            if (input != null)
            {
                try
                {
                    if (input.ToLower() == "shutdown")
                    {
                        Console.WriteLine("Shutting down...");
                        Sys.Power.Shutdown();
                    }
                    else if (input.ToLower() == "reboot")
                    {
                        Sys.Power.Reboot();
                    }
                    else if (input.ToLower() == "vol")
                    {
                        var vol = VFSManager.GetVolumes();
                        if (vol.Count > 0)
                        {
                            tablePrinter.WriteHeaders("Volume #", "File System", "Size");
                            foreach (var v in vol)
                            {
                                tablePrinter.WriteRow(v.mName, fileSystem.GetFileSystemType(v.mName), FormatSize(fileSystem.GetTotalSize(v.mName)));
                            }
                        }
                        else
                        {
                            Console.WriteLine("No volumes found");
                        }
                    }
                    else if (input.ToLower() == "dir")
                    {
                        var listing = fileSystem.GetDirectoryListing(currentPath);
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
                    else if (input.ToLower().StartsWith("cd"))
                    {
                        if (!input.Contains(" "))
                        {
                            Console.WriteLine("SYNTAX ERROR");
                        }
                        else currentPath = input.Split(" ")[1];
                    }
                    else
                    {
                        Console.WriteLine("SYNTAX ERROR");
                    }
                }
                catch (Exception e)
                {
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.BackgroundColor = ConsoleColor.Blue;
                    Console.WriteLine("Internal system error");
                    Console.WriteLine(e.ToString());
                    Console.ResetColor();
                }

                Console.WriteLine();

            }*/
        }
    }
}
