using System;
using System.Linq;
using Vintagestory.API.Common;

namespace CompatLib
{
    public static class ModChecker
    {
        /// <summary>
        /// Determines if a mod with the specified ID is loaded/enabled.
        /// </summary>
        public static bool IsModLoaded(ICoreAPI api, string modid)
        {
            try
            {
                return api.ModLoader.IsModEnabled(modid);
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Retrieves the version string of the specified mod if available; 
        /// returns "unknown" if not found or if no version is provided.
        /// </summary>
        public static string GetModVersion(ICoreAPI api, string modid)
        {
            try
            {
                var mod = api.ModLoader.GetMod(modid);
                var versionStr = mod?.Info?.Version;
                if (!string.IsNullOrWhiteSpace(versionStr))
                {
                    return versionStr;
                }
            }
            catch
            {
                // swallow exceptions and return "unknown"
            }
            return "unknown";
        }
    }
}
