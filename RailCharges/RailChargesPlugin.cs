using System;
using System.Collections.Generic;
using BepInEx;
using BepInEx.Logging;
using MonoMod.RuntimeDetour;
using RailCharges.Configuration;
using RailCharges.Internals;
using RailCharges.Internals.Debug;
using RailCharges.Patches;
using RailCharges.Patches.BackupMagCharges;
using RailCharges.Resources;
using RoR2;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace RailCharges
{
    [BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    public class RailChargesPlugin : BaseUnityPlugin
    {
        public static ManualLogSource Log;
        public static PluginConfig PluginConfig;
        public static CameraNudgeManager CameraNudgeManager;

        public static List<IPatch> Patches = new();

        private DetourModManager detourModManager;

        private void Awake()
        {
            Log = Logger;
            PluginConfig = PluginConfig.Load(this.Config);
            
            this.detourModManager = new DetourModManager();
            CurrentPlayerTracker.Init();
            CameraNudgeManager = new CameraNudgeManager();
            
#if DEBUG
            Logger.LogEvent += LoggerOnLogEvent;
            DisableAI.Init();
            Logger.LogInfo("Plugin is in DEBUG build");
#endif
            SetupPatches();

            ResourceManager.Init();
            SceneManager.sceneLoaded += OnSceneManagerOnsceneLoaded;

            // initialize everything if awoken in the middle of stage - i.e. when reloading when game is running
            if (Stage.instance)
            {
                Logger.LogInfo("Stage is running - patching");
                PatchAssets();
            }

            // Plugin startup logic
            Logger.LogInfo($"Plugin {PluginInfo.PLUGIN_GUID} is loaded!");
        }

        void SetupPatches()
        {
            Patches = new List<IPatch>
            {
                new LightReticleChange(),
                new FasterWindup(),
                new CameraScopeParametersOverride(),
                new DisableBoostedIndicator()
            };
            if (PluginConfig.DisplayBackupMagazineCharges) Patches.Add(new BackupMagChargesPatch());
            if (PluginConfig.AngularCompensation) Patches.Add(new AngularCompensationPatch(CameraNudgeManager));
        }

#if DEBUG
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.BackQuote))
            {
                Time.timeScale = Time.timeScale > 0 ? 0 : 1;
                Logger.LogInfo($"Time scale changed to {Time.timeScale}");
            }

            if (Input.GetKeyDown(KeyCode.End))
            {
                DisableAI.Disabled = !DisableAI.Disabled;
                Logger.LogInfo($"AI disabled: {DisableAI.Disabled}");
            }
        }
#endif

        private void OnSceneManagerOnsceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
        {
            PatchAssets();
        }

        public void PatchAssets()
        {
            foreach (var patch in Patches)
            {
                try
                {
                    patch.Patch(PluginConfig);
                }
                catch (Exception ex)
                {
                    Logger.LogError($"Error during patching: {ex}");
                }
            }
        }

        private void OnDestroy()
        {
            SceneManager.sceneLoaded -= OnSceneManagerOnsceneLoaded;
            this.detourModManager.Unload(typeof(RailChargesPlugin).Assembly);
#if DEBUG
            DebugCubes.Clear();
#endif
            foreach (var patch in Patches)
            {
                if (patch is IDisposable dis)
                {
                    dis.Dispose();
                }
            }
        }

        private void LoggerOnLogEvent(object sender, LogEventArgs e)
        {
            RoR2.Chat.AddMessage("" + e.Data);
        }
    }
}