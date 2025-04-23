using System.Collections.Concurrent;

namespace LSTY.Sdtd.ServerAdmin.Services.Core
{
    /// <summary>
    /// Manages the registration, unregistration, and retrieval of commands and their aliases.
    /// </summary>
    public class CommandRegistry
    {
        private readonly ConcurrentDictionary<string, CommandInfo> _commands = new(StringComparer.OrdinalIgnoreCase);
        private readonly ConcurrentDictionary<string, CommandInfo> _aliasMap = new(StringComparer.OrdinalIgnoreCase);

        /// <summary>
        /// Registers a new command and its aliases.
        /// </summary>
        /// <param name="command">The command to register.</param>
        /// <exception cref="ArgumentException">Thrown when the command name or any alias is already registered.</exception>
        public void RegisterCommand(CommandInfo command)
        {
            if (_commands.ContainsKey(command.Name))
                throw new ArgumentException($"Command name '{command.Name}' is already registered.");

            foreach (var alias in command.Aliases)
            {
                if (_aliasMap.ContainsKey(alias) || _commands.ContainsKey(alias))
                    throw new ArgumentException($"Alias '{alias}' is already registered.");
            }

            _commands[command.Name] = command;

            foreach (var alias in command.Aliases)
            {
                _aliasMap[alias] = command;
            }
        }

        /// <summary>
        /// Unregisters a command and its aliases.
        /// </summary>
        /// <param name="name">The name of the command to unregister.</param>
        /// <returns>True if the command was successfully unregistered; otherwise, false.</returns>
        public bool UnregisterCommand(string? name)
        {
            if (string.IsNullOrEmpty(name))
                return false;

            if (_commands.TryGetValue(name, out var command) == false)
                return false;

            // Remove the command
            _commands.TryRemove(command.Name, out _);

            // Remove all aliases
            foreach (var alias in command.Aliases)
            {
                _aliasMap.TryRemove(alias, out _);
            }

            return true;
        }

        /// <summary>
        /// Attempts to retrieve a command by its name or alias.
        /// </summary>
        /// <param name="name">The name or alias of the command.</param>
        /// <param name="command">The retrieved command, or null if not found.</param>
        /// <returns>True if the command was found; otherwise, false.</returns>
        public bool TryGetCommand(string name, out CommandInfo? command)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                command = null;
                return false;
            }

            if (_commands.TryGetValue(name, out command))
                return true;

            if (_aliasMap.TryGetValue(name, out command))
                return true;

            command = null;
            return false;
        }

        /// <summary>
        /// Retrieves all registered commands.
        /// </summary>
        /// <returns>An enumerable collection of all registered commands.</returns>
        public IEnumerable<CommandInfo> GetAllCommands() => _commands.Values;
    }
}
