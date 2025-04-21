namespace LSTY.Sdtd.ServerAdmin.Services.Core
{
    public class CommandInfo
    {
        public required string Name { get; set; }
        public required IEnumerable<string> Aliases { get; set; }
        public required string Description { get; set; }
        public required Func<string[], CommandSender, Task> Execute { get; set; }
    }
}
