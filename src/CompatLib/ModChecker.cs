using System;
using System.Collections.Concurrent;
using System.Linq;
using Vintagestory.API.Common;

namespace CompatLib
{
    /// <summary>
    /// Provides utility methods to check the presence and version of mods within the game.
    /// </summary>
    public static class ModChecker
    {
        /// <summary>
        /// Determines whether a mod with the specified identifier is currently loaded and enabled.
        /// </summary>
        /// <param name="api">The core API instance of the game.</param>
        /// <param name="modid">The unique identifier of the mod to check.</param>
        /// <returns><c>true</c> if the mod is enabled; otherwise, <c>false</c>.</returns>
        public static bool IsModLoaded(ICoreAPI api, string modid)
        {
            try
            {
                return api.ModLoader.IsModEnabled(modid);
            }
            catch (Exception ex)
            {
                api.Logger.Warning($"Error checking mod '{modid}': {ex.Message}");
                return false;
            }
        }

        private static readonly ConcurrentDictionary<string, string> ModVersionCache = new();
        /// <summary>
        /// Retrieves the version string of the specified mod.
        /// </summary>
        /// <param name="api">The core API instance of the game.</param>
        /// <param name="modid">The unique identifier of the mod whose version is to be retrieved.</param>
        /// <returns>The version string of the mod if available; otherwise, "unknown".</returns>
        public static string GetModVersion(ICoreAPI api, string modid)
        {
            if (ModVersionCache.TryGetValue(modid, out var cachedVersion))
                return cachedVersion;
            try
            {
                var mod = api.ModLoader.GetMod(modid);
                var versionStr = mod?.Info?.Version;
                if (!string.IsNullOrWhiteSpace(versionStr))
                {
                    ModVersionCache[modid] = versionStr;
                    return versionStr;
                }
            }
            catch (Exception ex)
            {
                api.Logger.Warning($"Error retrieving version for mod '{modid}': {ex.Message}");
            }
            return "unknown";
        }
    }
}
