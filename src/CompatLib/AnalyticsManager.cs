using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text.Json;

namespace CompatLib
{
    /// <summary>
    /// Provides functionality to track and log compatibility conflicts within the CompatLib framework.
    /// </summary>
    public static class AnalyticsManager
    {
        /// <summary>
        /// A thread-safe queue that stores log messages related to compatibility conflicts.
        /// </summary>
        private static readonly ConcurrentQueue<string> conflictLogs = new();

        /// <summary>
        /// Gets the total number of recorded compatibility conflicts.
        /// </summary>
        public static int ConflictCount => conflictLogs.Count;

        /// <summary>
        /// Gets an enumerable collection of all logged compatibility conflict messages.
        /// </summary>
        public static IEnumerable<string> GetConflictLogs()
        {
            return conflictLogs.ToArray();
        } 

        /// <summary>
        /// Logs a compatibility conflict message with a timestamp.
        /// </summary>
        /// <param name="message">The conflict message to log.</param>
        public static void LogConflict(string message)
        {
            conflictLogs.Enqueue($"{DateTime.UtcNow}: {message}");
        }

        /// <summary>
        /// Logs an info message with a timestamp.
        /// </summary>
        /// <param name="message">The conflict message to log.</param>
        public static void LogInfo(string message)
        {
            conflictLogs.Enqueue($"{DateTime.UtcNow} [INFO]: {message}");
        }

        /// <summary>
        /// Logs a warning message with a timestamp.
        /// </summary>
        /// <param name="message">The conflict message to log.</param>
        public static void LogWarning(string message)
        {
            conflictLogs.Enqueue($"{DateTime.UtcNow} [WARNING]: {message}");
        }

        /// <summary>
        /// Logs an error message with a timestamp.
        /// </summary>
        /// <param name="message">The conflict message to log.</param>
        public static void LogError(string message)
        {
            conflictLogs.Enqueue($"{DateTime.UtcNow} [ERROR]: {message}");
        }

        /// <summary>
        /// Exports the collected conflict logs to a JSON file.
        /// </summary>
        /// <param name="filePath">The file path where the JSON log will be saved.</param>
        public static void ExportLogsToJson(string filePath)
        {
            try
            {
                var logs = conflictLogs.ToArray();
                var jsonOptions = new JsonSerializerOptions { WriteIndented = true };
                var jsonString = JsonSerializer.Serialize(logs, jsonOptions);
                File.WriteAllText(filePath, jsonString);
            }
            catch (Exception ex)
            {
                // Handle exceptions such as file I/O errors
                Console.WriteLine($"An error occurred while exporting logs to JSON: {ex.Message}");
            }
        }
    }
}
