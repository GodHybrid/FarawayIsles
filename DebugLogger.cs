using StardewModdingAPI;
using FarawayIsles;

namespace DebugLogger
{
    public static class DebugLogger
    {
        /// <summary>
        /// Log to DEBUG if compiled with DEBUG
        /// Log to verbose only otherwise.
        /// </summary>
        /// <param name="message">Message to log.</param>
        public static void DebugLog(string message, LogLevel level = LogLevel.Debug)
        {
        #if DEBUG
            ModEntry.Instance.Monitor.Log(message, level);
        #else
            ModEntry.Instance.Monitor.VerboseLog(message);
        #endif
        }
    }
}