using System.Drawing;
using Cosmos.System.Graphics;

namespace LineOS.CLI.Commands
{
    public class CmdGui : ICommand
    {
        public string Name => "gui";
        public string Description => "Starts the GUI for LineOS";
        public string Syntax => "gui";

        public bool Execute(string[] args)
        {
            var canvas = FullScreenCanvas.GetFullScreenCanvas(new Mode(1366, 768, ColorDepth.ColorDepth32));
            canvas.Clear(Color.Green);
            return true;
        }
    }
}
