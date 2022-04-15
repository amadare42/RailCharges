using System;
using BepInEx;
using BepInEx.Logging;
using RoR2;

namespace RailCharges
{
    [BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    public class Plugin : BaseUnityPlugin
    {
        public static ManualLogSource Log;

        public static PlayerCharacterMasterController LocalPlayerCharacterMasterController;

        private void Awake()
        {
            Log = Logger;
            
            On.EntityStates.Railgunner.Scope.ActiveScopeHeavy.ctor += (orig, self) =>
            {
                try
                {
                    orig(self);

                    if (!self.crosshairOverridePrefab.GetComponent<ScopeTracker>())
                    {
                        self.crosshairOverridePrefab.AddComponent<ScopeTracker>();
                        ScopeTracker.PatchPrefab(self.crosshairOverridePrefab);
                        Logger.LogInfo($"Crosshair prefab patched!");
                    }
                }
                catch (Exception ex)
                {
                    Logger.LogError(ex);
                }
            };
            
            On.RoR2.CharacterBody.OnSkillActivated += (orig, self, skill) =>
            {
                try
                {
                    orig(self, skill);

                    if (self.hasEffectiveAuthority && skill.skillNameToken is "RAILGUNNER_SNIPE_HEAVY_NAME")
                    {
                        ScopeTracker.UpdateCharges(self.skillLocator.primary);
                    }
                }
                catch (Exception ex)
                {
                    Logger.LogError(ex);
                }
            };

            On.RoR2.PlayerCharacterMasterController.OnBodyStart += (orig, self) =>
            {
                try
                {
                    orig(self);
                    if (self.networkUser.isLocalPlayer)
                    {
                        LocalPlayerCharacterMasterController = self;
                    }
                }
                catch (Exception ex)
                {
                    Logger.LogError(ex);
                }
            };

            // Plugin startup logic
            Logger.LogInfo($"Plugin {PluginInfo.PLUGIN_GUID} is loaded!");
        }
    }
}
