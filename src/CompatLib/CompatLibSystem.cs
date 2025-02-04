using System;
using System.Linq;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Datastructures;
using Vintagestory.API.Server;

namespace CompatLib
{
    /// <summary>
    /// Represents the main mod system for CompatLib, responsible for initializing the library and processing compatibility handlers.
    /// </summary>
    public class CompatLibSystem : ModSystem
    {
        /// <summary>
        /// Called when the mod system starts. Initializes the mod system, registers commands, and logs the initialization process.
        /// </summary>
        /// <param name="api">The core API instance of the game.</param>
        public override void Start(ICoreAPI api)
        {
            base.Start(api);
            api.Logger.Notification("CompatLib initialized for mod compatibility.");
            RegisterCommands(api);
        }

        /// <summary>
        /// Registers chat commands for displaying compatibility conflict logs.
        /// </summary>
        /// <param name="api">The core API instance of the game.</param>
        private void RegisterCommands(ICoreAPI api)
        {
            // Client-side command registration
            if (api is ICoreClientAPI capi)
            {
                capi.ChatCommands
                    .Create("compatlog")
                    .WithDescription("Display compatibility conflict logs.")
                    .HandleWith(args =>
                    {
                        if (AnalyticsManager.ConflictCount == 0)
                        {
                            capi.ShowChatMessage("No compatibility conflicts recorded.");
                            return TextCommandResult.Success();
                        }
                        foreach (var log in AnalyticsManager.GetConflictLogs())
                        {
                            capi.ShowChatMessage(log);
                        }
                        return TextCommandResult.Success();
                    });
            }

            // Server-side command registration
            if (api is ICoreServerAPI sapi)
            {
                sapi.ChatCommands
                    .Create("compatlog")
                    .WithDescription("Display compatibility conflict logs.")
                    .RequiresPrivilege(Privilege.chat)
                    .HandleWith((args) =>
                    {
                        IServerPlayer? player = args.Caller as IServerPlayer;
                        if (AnalyticsManager.ConflictCount == 0)
                        {
                            sapi.Logger.Notification("No compatibility conflicts recorded.");
                            return TextCommandResult.Success();
                        }
                        foreach (var log in AnalyticsManager.GetConflictLogs())
                        {
                            sapi.Logger.Notification(log);
                        }
                        return TextCommandResult.Success();
                    });
            }
        }

        /// <summary>
        /// Called when the mod system starts on the server side. Processes compatibility handlers for each registered mod.
        /// </summary>
        /// <param name="api">The server API instance of the game.</param>
        public override void StartServerSide(ICoreServerAPI api)
        {
            api.Logger.Notification("CompatLib (server) active.");
            var modGroups = CompatibilityManager.GetRegistrations().GroupBy(r => r.TargetModId);
            foreach (var group in modGroups)
            {
                string modid = group.Key;
                if (ModChecker.IsModLoaded(api, modid))
                {
                    CompatibilityManager.ProcessHandlers(modid, api);
                }
                else
                {
                    api.Logger.Notification($"Mod '{modid}' not detected; skipping compatibility handlers.");
                }
            }
        }

        /// <summary>
        /// Called when the mod system starts on the client side. Logs the activation of CompatLib on the client.
        /// </summary>
        /// <param name="api">The client API instance of the game.</param>
        public override void StartClientSide(ICoreClientAPI api)
        {
            api.Logger.Notification("CompatLib (client) active.");
        }
    }
}
