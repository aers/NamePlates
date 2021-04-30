using System;
using Dalamud.Plugin;
using NamePlates.Attributes;
using NamePlates.Hooks;

namespace NamePlates
{
    public class Plugin : IDalamudPlugin
    {
        public DalamudPluginInterface pluginInterface;
        private PluginCommandManager<Plugin> commandManager;
        private Configuration config;
        private PluginUI ui;

        private AddonNamePlateHooks npHooks;

        public string Name => "NamePlates";

        public void Initialize(DalamudPluginInterface pluginInterface)
        {
            this.pluginInterface = pluginInterface;

            this.config = (Configuration)this.pluginInterface.GetPluginConfig() ?? new Configuration();
            this.config.Initialize(this.pluginInterface);

            this.ui = new PluginUI();
            this.pluginInterface.UiBuilder.OnBuildUi += this.ui.Draw;

            this.commandManager = new PluginCommandManager<Plugin>(this, this.pluginInterface);

            npHooks = new AddonNamePlateHooks(this);
            npHooks.Initialize();
        }

        [Command("/npc")]
        [HelpMessage("Show NamePlates config.")]
        public void ShowConfig(string command, string args)
        {
            this.ui.IsVisible = true;
        }

        #region IDisposable Support
        protected virtual void Dispose(bool disposing)
        {
            if (!disposing) return;

            this.npHooks.Dispose();

            this.commandManager.Dispose();

            this.pluginInterface.SavePluginConfig(this.config);

            this.pluginInterface.UiBuilder.OnBuildUi -= this.ui.Draw;

            this.pluginInterface.Dispose();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}
