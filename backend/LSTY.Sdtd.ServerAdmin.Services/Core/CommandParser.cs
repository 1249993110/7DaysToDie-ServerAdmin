using System.Text.RegularExpressions;

namespace LSTY.Sdtd.ServerAdmin.Services.Core
{
    /// <summary>
    /// A utility class for parsing commands from chat text. Supports multiple command prefixes, argument separators, and command alias mapping.
    /// </summary>
    public class CommandParser
    {
        private readonly char[] _prefixes;
        private readonly bool _allowNoPrefix;
        private readonly char[] _argSeparators;

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandParser"/> class.
        /// </summary>
        /// <param name="commandPrefixes">Command prefixes, such as '/' or '!'.</param>
        /// <param name="allowNoPrefix">If true, allows commands without a prefix.</param>
        /// <param name="argSeparators">Argument separators, such as spaces, commas, or semicolons.</param>
        public CommandParser(char[]? commandPrefixes, bool allowNoPrefix, char[]? argSeparators)
        {
            _prefixes = commandPrefixes ?? new char[] { '/', '!', '@', '~', ':' };
            _allowNoPrefix = allowNoPrefix;
            _argSeparators = argSeparators ?? new char[] { ' ', '\t', '-' };
        }

        /// <summary>
        /// Parses the input string to extract the command name and arguments.
        /// </summary>
        /// <param name="input">The input string to parse.</param>
        /// <returns>
        /// A <see cref="CommandParseResult"/> object containing the parsing result, including whether the input is a command,
        /// the command name, and the arguments.
        /// </returns>
        public CommandParseResult Parse(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return new CommandParseResult(false);

            input = input.Trim();

            bool hasPrefix = Array.IndexOf(_prefixes, input[0]) >= 0;

            if (hasPrefix == false && _allowNoPrefix == false)
                return new CommandParseResult(false);

            string commandLine = hasPrefix ? input.Substring(1) : input;

            string[] tokens;
            if (commandLine.Contains('"') == false)
            {
                tokens = commandLine.Split(_argSeparators, StringSplitOptions.RemoveEmptyEntries);
            }
            else
            {
                tokens = ParseWithQuotes(commandLine);
            }

            if (tokens.Length == 0)
                return new CommandParseResult(false, null, null);

            string cmdName = tokens[0];
            string[] args = tokens.Length > 1 ? tokens[1..] : Array.Empty<string>();

            return new CommandParseResult(true, cmdName, args);
        }

        private string[] ParseWithQuotes(string input)
        {
            // Use regular matching with quoted parameters (with quotes as a whole) and support multi delimiter splitting
            var matches = Regex.Matches(input, @"[\""].+?[\""]|[^" + Regex.Escape(new string(_argSeparators)) + @"]+");

            if (matches.Count == 0)
                return Array.Empty<string>();

            var tokens = new List<string>();
            foreach (Match match in matches)
            {
                string token = match.Value.Trim();
                if (string.IsNullOrEmpty(token))
                {
                    continue;
                }

                // Remove surrounding quotes if present
                if (token.StartsWith('\"') && token.EndsWith('\"') && token.Length >= 2)
                {
                    token = token.Substring(1, token.Length - 2);
                }

                tokens.Add(token);
            }

            return tokens.ToArray();
        }
    }
}
