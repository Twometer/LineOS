namespace LineOS.CLI.Commands
{
    public class CmdChgDir : ICommand
    {
        public string Name => "cd";

        public string Description => "Change current path";

        public string Syntax => "cd <path>";

        public bool Execute(string[] args)
        {
            if (args.Length != 1) return false;

            var cdArg = "";
            foreach (var a in args)
                cdArg += a + " ";
            cdArg = cdArg.Trim();

            var newPath = Kernel.ConsoleManager.CurrentPath;

            if (cdArg.Contains(":"))
                newPath = cdArg;
            else if (cdArg.StartsWith("\\"))
                newPath = newPath.Remove(2) + cdArg;
            else if (cdArg == "..")
            {
                if (!newPath.EndsWith("\\"))
                    newPath = newPath.Remove(LastIndexOf(newPath, '\\'));
            }
            else
                newPath += (newPath.EndsWith("\\") ? "" : "\\") + cdArg;

            CheckPath(newPath);
            Kernel.ConsoleManager.CurrentPath = newPath;

            return true;
        }

        private static int LastIndexOf(string str, char chr)
        {
            for (var i = str.Length - 1; i >= 0; i--)
                if (str[i] == chr)
                    return i;
            return -1;
        }

        private static void CheckPath(string path)
        {
            Kernel.FileSystem.GetDirectoryListing(path);
        }
    }
}
