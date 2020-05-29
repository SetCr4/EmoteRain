using System.Collections.Generic;
using static EmoteRain.Logger;

namespace EmoteRain.Commands
{
    public class CommandRegistration
    {
        internal static Dictionary<string, ERCommand> registeredCommands = new Dictionary<string, ERCommand>();
        public static void registerCommands()
        {
            IEnumerable<ERCommand> commands = Extensions.GetEnumerableOfType<ERCommand>();
            foreach (ERCommand e in commands)
            {
                registeredCommands.Add(e.trigger, e);
            }
            Log($"{registeredCommands.Values.Count} registered commands");
        }
    }
}
