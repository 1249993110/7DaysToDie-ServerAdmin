namespace LSTY.Sdtd.ServerAdmin.Services.Core
{
    public class CommandParseResult
    {
        public bool IsCommand { get; set; }
        public string CommandName { get; set; }
        public string[] Arguments { get; set; }

        public CommandParseResult(bool isCommand, string? commandName = null, string[]? arguments = null)
        {
            IsCommand = isCommand;
            CommandName = commandName ?? string.Empty;
            Arguments = arguments ?? Array.Empty<string>();
        }
    }
}
