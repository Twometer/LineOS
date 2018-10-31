using Cosmos.System.FileSystem.VFS;
using System;

namespace LineOS.CLI.Commands
{
    public class CmdVolumeList : ICommand
    {
        public string Name => "vol";

        public string Description => "List all available volumes";

        public string Syntax => "vol";

        public bool Execute(string[] args)
        {
            var vol = VFSManager.GetVolumes();
            if (vol.Count > 0)
            {
                Kernel.TablePrinter.WriteHeaders("Volume #", "File System", "Size");
                foreach (var v in vol)
                    Kernel.TablePrinter.WriteRow(v.mName, Kernel.FileSystem.GetFileSystemType(v.mName), FormatSize(Kernel.FileSystem.GetTotalSize(v.mName)));
            }
            else
                Console.WriteLine("No volumes found");
            return true;
        }

        private string FormatSize(long bytes)
        {
            string[] units = { "B", "KB", "MB", "GB", "TB" };
            for (int i = 0; i < units.Length; i++)
            {
                long max = (long)Math.Pow(1024, i + 1);
                if (bytes < max)
                    return (int)(bytes / Math.Pow(1024, i)) + " " + units[i];
            }

            return bytes + " B";
        }
    }
}
