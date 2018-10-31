namespace LineOS.CLI
{
    public interface ICommand
    {
        string Name { get; }
        string Description { get; }
        string Syntax { get; }

        bool Execute(string[] args);
    }
}
