using System;
using System.Text;

namespace LineOS.CLI.Commands
{
    public class CmdReadFile : ICommand
    {
        public string Name => "rdfile";
        public string Description => "Read the contents of a file";
        public string Syntax => "rdfile <path>";

        public bool Execute(string[] args)
        {
            if (args.Length != 1) return false;

            var stream = Kernel.FileSystem.GetFile(args[0]).GetFileStream();
            var arr = new byte[512];
            stream.Read(arr, 0, arr.Length);
            Console.WriteLine(Encoding.ASCII.GetString(arr));
            return true;
        }
    }
}
