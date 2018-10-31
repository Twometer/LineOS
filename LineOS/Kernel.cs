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

            Console.Clear();

            Console.WriteLine("LineOS 1.0.0");
            Console.WriteLine("(c) 2018 Twometer Applications");
            Console.WriteLine();
        }

        protected override void Run()
        { 
          ConsoleManager.HandleConsole();
        }
    }
}
