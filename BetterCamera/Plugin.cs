using System.Reflection;
using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
using UnityEngine;

namespace BetterCamera
{
    [BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
    public class BetterCamera : BaseUnityPlugin
    {
        public static BetterCamera Instance { get; private set; }
        public ConfigEntry<bool> IsEnabled { get; private set; }

        private Harmony _harmony;

        private void Awake()
        {
            Instance = this;

            IsEnabled = Config.Bind(
                section: MyPluginInfo.PLUGIN_NAME,
                key: "Enabled",
                defaultValue: true,
                description: "Enable or disable the BetterCamera mod"
            );

            Logger.LogInfo($"{MyPluginInfo.PLUGIN_NAME} v{MyPluginInfo.PLUGIN_VERSION} loaded. Enabled: {IsEnabled.Value}");

            _harmony = new Harmony(MyPluginInfo.PLUGIN_GUID);
            _harmony.PatchAll(Assembly.GetExecutingAssembly());
        }

        private void OnDestroy()
        {
            // <— here we call UnpatchSelf() instead of UnpatchAll(string)
            _harmony?.UnpatchSelf();

            Logger.LogInfo($"{MyPluginInfo.PLUGIN_NAME} unloaded");
            Instance = null;
        }
    }
}
