using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Vintagestory.API.Common;

namespace CompatLib
{
    /// <summary>
    /// Represents a registered compatibility handler for a specific target mod.
    /// </summary>
    public class CompatibilityRegistration
    {
        /// <summary>
        /// Gets or sets the unique identifier of the target mod for which this handler is registered.
        /// </summary>
        public required string TargetModId { get; set; }

        /// <summary>
        /// Gets or sets the priority of this handler. Higher values indicate higher priority.
        /// </summary>
        public int Priority { get; set; }

        /// <summary>
        /// Gets or sets the delegate that defines the compatibility handling logic.
        /// </summary>
        public CompatibilityHandler? Handler { get; set; }

        /// <summary>
        /// Gets or sets a brief description of this compatibility handler.
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// Gets or sets a list of mod IDs that this handler depends on.
        /// </summary>
        public List<string> Dependences { get; set; } = new();

        /// <summary>
        /// Gets or sets a value indicating whether this handler is mutually exclusive with other handlers.
        /// </summary>
        public bool IsMutuallyExclusive { get; set; } = false;

        /// <summary>
        /// Gets or sets a value indicating whether this handler is enabled.
        /// </summary>
        public bool IsEnabled { get; set; } = true;

        /// <summary>
        /// Gets or sets the version of the target mod that this handler is compatible with.
        /// </summary>
        public string? CompatibleVersion { get; set; }
    }

    /// <summary>
    /// Delegate type for methods that handle compatibility logic.
    /// </summary>
    public delegate void CompatibilityHandler();

    /// <summary>
    /// Manages registration and execution of compatibility handlers for various mods.
    /// </summary>
    public static class CompatibilityManager
    {
        /// <summary>
        /// A thread-safe dictionary storing lists of compatibility registrations, keyed by target mod ID.
        /// </summary>
        private static readonly ConcurrentDictionary<string, List<CompatibilityRegistration>> registrations = new();

        /// <summary>
        /// Gets or sets a dictionary that allows specifying override handlers for specific mods.
        /// </summary>
        public static ConcurrentDictionary<string, string> OverrideHandlers {  get; set; } = new();

        /// <summary>
        /// Registers a compatibility handler for a specified target mod.
        /// </summary>
        /// <param name="targetModId">The unique identifier of the target mod.</param>
        /// <param name="priority">The priority of the handler; higher values indicate higher priority.</param>
        /// <param name="handler">The delegate that contains the compatibility handling logic.</param>
        /// <param name="description">A brief description of the handler.</param>
        /// <param name="mutuallyExclusive">Indicates whether this handler is mutually exclusive with other handlers.</param>
        /// <param name="dependencies">A list of mod IDs that this handler depends on.</param>
        /// <param name="compatibleVersion">The version of the target mod that this handler is compatible with.</param>
        public static void RegisterHandler(
            string targetModId,
            int priority,
            CompatibilityHandler handler,
            string description = "",
            bool mutuallyExclusive = false,
            List<string>? dependencies = null,
            string? compatibileVersion = null)
        {
            var registration = new CompatibilityRegistration
            {
                TargetModId = targetModId,
                Priority = priority,
                Handler = handler,
                Description = description,
                IsMutuallyExclusive = mutuallyExclusive,
                Dependences = dependencies ?? new List<string>(),
                CompatibleVersion = compatibileVersion
            };

            registrations.AddOrUpdate(
                targetModId,
                new List<CompatibilityRegistration> { registration },
                (key, existingList) =>
                {
                    existingList.Add(registration);
                    return existingList;
                });
        }

        /// <summary>
        /// Processes and executes all applicable handlers registered for a given mod.
        /// </summary>
        /// <param name="targetModId">The unique identifier of the target mod.</param>
        /// <param name="api">The core API instance of the game.</param>
        public static void ProcessHandlers(string targetModId, ICoreAPI api)
        {
            if (!registrations.TryGetValue(targetModId, out var handlers) || handlers.Count == 0)
            {
                api.Logger.Warning($"No handlers registered for mod '{targetModId}'.");
                return;
            }

            // Filter out disabled handlers and sort by priority in descending order
            handlers = handlers.Where(h => h.IsEnabled).OrderByDescending(r => r.Priority).ToList();

            // Check for mutual exculusivity conflicts
            if (handlers.Any(h => h.IsMutuallyExclusive) && handlers.Count > 1)
            {
                string conflictMessage = $"Conflict detected for mod '{targetModId}': {handlers.Count} handlers registered.";
                AnalyticsManager.LogConflict(conflictMessage);
                api.Logger.Warning(conflictMessage);
                return;
            }

            // Apply override if specified
            if (OverrideHandlers.TryGetValue(targetModId, out string? overrideHandler))
            {
                var overriden = handlers.FirstOrDefault(h => h.Description == overrideHandler);
                if (overriden != null)
                {
                    ExecuteHandler(overriden, api);
                    api.Logger.Notification($"Override handler for mod '{targetModId}' applied.");
                }
                else
                {
                    api.Logger.Warning($"Override handler '{overrideHandler}' for mod '{targetModId}' not found.");
                }
                return;
            }

            // Execute all handlers sequentially
            foreach (var handler in handlers )
            {
                if (handler.CompatibleVersion != null)
                {
                    string modVersion = ModChecker.GetModVersion(api, targetModId);
                    if (modVersion != handler.CompatibleVersion)
                    {
                        api.Logger.Warning($"Handler for mod '{targetModId}' is not compatible with version {modVersion}. Expected version: {handler.CompatibleVersion}.");
                        continue;
                    }
                }

                foreach (var dependency in handler.Dependences)
                {
                    if (registrations.ContainsKey(dependency))
                    {
                        ProcessHandlers(dependency, api);
                    }
                    else
                    {
                        api.Logger.Warning($"Dependency '{dependency}' for mod '{targetModId}' not found.");
                    }
                }
                ExecuteHandler(handler, api);
                api.Logger.Notification($"Executing {handlers.Count} handlers for mod '{targetModId}'.");
            }
        }

        /// <summary>
        /// Executes a specified compatibility handler and logs the outcome.
        /// </summary>
        /// <param name="handler">The compatibility registration to execute.</param>
        /// <param name="api">The core API instance of the game.</param>
        private static void ExecuteHandler(CompatibilityRegistration handler, ICoreAPI api)
        {
            try
            {
                handler.Handler?.Invoke();
                api.Logger.Notification($"Handler for mod '{handler.TargetModId}' with priority {handler.Priority} executed.");
            }
            catch (Exception ex)
            {
                string errorMessage = $"Error executing handler for mod '{handler.TargetModId}': {ex.Message}";
                AnalyticsManager.LogConflict(errorMessage);
                api.Logger.Warning(errorMessage);
            }
        }

        /// <summary>
        /// Returns all registered compatibility registrations.
        /// </summary>
        /// <returns>An enumerable collection of all <see cref="CompatibilityRegistration"/> instances.</returns>
        public static IEnumerable<CompatibilityRegistration> GetRegistrations()
        {
            return registrations.Values.SelectMany(r => r);
        }

        /// <summary>
        /// Sets the enabled state of a specific handler for a target mod.
        /// </summary>
        /// <param name="targetModId">The unique identifier of the target mod.</param>
        /// <param name="description">The description of the handler to modify.</param>
        /// <param name="isEnabled">A boolean value indicating whether the handler should be enabled or disabled.</param>
        public static void SetHandlerEnabled(string targetModId, string description, bool isEnabled)
        {
            if (registrations.TryGetValue(targetModId, out var handlers))
            {
                var handler = handlers.FirstOrDefault(h => h.Description == description);
                if (handler != null)
                {
                    handler.IsEnabled = isEnabled;
                }
            }
        }
    }
}
