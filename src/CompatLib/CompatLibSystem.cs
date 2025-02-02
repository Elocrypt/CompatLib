using System;
using System.Linq;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Datastructures;
using Vintagestory.API.Server;

namespace CompatLib
{
    /// <summary>
    /// Main mod system for CompatLib.
    /// Initializes the library and processes compatibility handlers.
    /// </summary>
    public class CompatLibSystem : ModSystem
    {
        public override void Start(ICoreAPI api)
        {
            base.Start(api);
            api.Logger.Notification("CompatLib initialized for mod compatibility.");
            RegisterCommands(api);
        }

        private void RegisterCommands(ICoreAPI api)
        {
            // -----------------------------------
            // CLIENT SIDE
            // -----------------------------------
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
                        foreach (var log in AnalyticsManager.ConflictLogs)
                        {
                            capi.ShowChatMessage(log);
                        }
                        return TextCommandResult.Success();
                    });
            }

            // -----------------------------------
            // SERVER SIDE
            // -----------------------------------
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
                        foreach (var log in AnalyticsManager.ConflictLogs)
                        {
                            sapi.Logger.Notification(log);
                        }
                        return TextCommandResult.Success();
                    });
            }
        }

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

        public override void StartClientSide(ICoreClientAPI api)
        {
            api.Logger.Notification("CompatLib (client) active.");
        }
    }
}
