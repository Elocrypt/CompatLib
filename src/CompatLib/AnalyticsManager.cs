using System;
using System.Collections.Generic;

namespace CompatLib
{
    /// <summary>
    /// Tracks analytics for compatibility conflicts.
    /// </summary>
    public static class AnalyticsManager
    {
        public static int ConflictCount { get; private set; } = 0;
        public static List<string> ConflictLogs { get; private set; } = new List<string>();

        /// <summary>
        /// Logs a conflict message and increments the conflict counter.
        /// </summary>
        public static void LogConflict(string message)
        {
            ConflictCount++;
            ConflictLogs.Add($"{DateTime.UtcNow}: {message}");
        }
    }
}
