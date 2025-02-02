using System;
using System.Collections.Generic;
using System.Linq;
using Vintagestory.API.Common;

namespace CompatLib
{
    /// <summary>
    /// Represents a registered compatibility handler.
    /// </summary>
    public class CompatibilityRegistration
    {
        public required string TargetModId { get; set; }
        public int Priority { get; set; }
        public CompatibilityHandler? Handler { get; set; }
        public string? Description { get; set; }
    }

    /// <summary>
    /// Delegate type for compatibility handler callbacks.
    /// </summary>
    public delegate void CompatibilityHandler();

    /// <summary>
    /// Manages registration and processing of compatibility handlers.
    /// </summary>
    public static class CompatibilityManager
    {
        // Key: Target mod id; Value: List of handlers registered for that mod.
        private static Dictionary<string, List<CompatibilityRegistration>> registrations = new Dictionary<string, List<CompatibilityRegistration>>();

        /// <summary>
        /// Registers a compatibility handler for a given target mod.
        /// </summary>
        public static void RegisterHandler(string targetModId, int priority, CompatibilityHandler handler, string description = "")
        {
            if (!registrations.ContainsKey(targetModId))
            {
                registrations[targetModId] = new List<CompatibilityRegistration>();
            }
            registrations[targetModId].Add(new CompatibilityRegistration
            {
                TargetModId = targetModId,
                Priority = priority,
                Handler = handler,
                Description = description
            });
        }

        /// <summary>
        /// Processes all handlers registered for a given mod. In case of multiple handlers, the one with the highest priority is executed.
        /// </summary>
        public static void ProcessHandlers(string targetModId, ICoreAPI api)
        {
            if (!registrations.ContainsKey(targetModId))
            {
                return;
            }

            // Order handlers by descending priority.
            var handlers = registrations[targetModId].OrderByDescending(r => r.Priority).ToList();

            // If more than one handler is registered, log a conflict.
            if (handlers.Count > 1)
            {
                string conflictMessage = $"Conflict detected for mod '{targetModId}': {handlers.Count} handlers registered. Defaulting to highest priority.";
                AnalyticsManager.LogConflict(conflictMessage);
                api.Logger.Warning(conflictMessage);
            }

            // Invoke the highest priority handler.
            var chosen = handlers.First();
            try
            {
                chosen.Handler?.Invoke();
                api.Logger.Notification($"Compatibility handler for mod '{targetModId}' applied with priority {chosen.Priority}.");
            }
            catch (Exception ex)
            {
                string errorMessage = $"Error applying compatibility handler for mod '{targetModId}': {ex.Message}";
                AnalyticsManager.LogConflict(errorMessage);
                api.Logger.Error(errorMessage);
            }
        }

        /// <summary>
        /// Returns all registered compatibility registrations.
        /// </summary>
        public static IEnumerable<CompatibilityRegistration> GetRegistrations()
        {
            return registrations.Values.SelectMany(r => r);
        }
    }
}
