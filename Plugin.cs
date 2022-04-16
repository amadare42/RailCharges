using System;
using System.Linq;
using BepInEx;
using BepInEx.Logging;
using EntityStates.Railgunner.Scope;
using RoR2;
using RoR2.UI;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace RailCharges
{
    [BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    public class Plugin : BaseUnityPlugin
    {
        public static ManualLogSource Log;
        public static CharacterBody LocalPlayerBody;

        private void Awake()
        {
            Log = Logger;
            
            SceneManager.sceneLoaded += (_, _) => PatchAssets();
            
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
                        Logger.LogInfo("Player controller found");
                        LocalPlayerBody = self.master.GetBody();
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

        public void PatchAssets()
        {
            try
            {
                var configuration = UnityEngine.AddressableAssets.Addressables
                    .LoadAssetAsync<RoR2.EntityStateConfiguration>("RoR2/DLC1/Railgunner/EntityStates.Railgunner.Scope.ActiveScopeHeavy.asset")
                    .WaitForCompletion();
                
                var crosshairPrefab = (GameObject)configuration.serializedFieldsCollection.serializedFields
                    .First(sf => sf.fieldName == nameof(ActiveScopeHeavy.crosshairOverridePrefab)).fieldValue
                    .objectValue;

                if (!crosshairPrefab.GetComponent<ScopeTracker>())
                {
                    crosshairPrefab.AddComponent<ScopeTracker>();
                }

                PatchPrefab(crosshairPrefab);
                
                Logger.LogInfo($"Crosshair prefab patched!");
            }
            catch (Exception ex)
            {
                Logger.LogError($"Error during patching scope asset configuration: {ex}");
            }
        }

        public static void PatchPrefab(GameObject prefab)
        {
            var go = new GameObject("charges");
        
            var textMesh = go.AddComponent<HGTextMeshProUGUI>();
            textMesh.alpha = .1f;
            textMesh.fontSize = 30;

            var rectTransform = (RectTransform)go.transform;
            rectTransform.anchoredPosition = new Vector2(50, 0);
            rectTransform.offsetMin = new Vector2(5, 0);
            rectTransform.offsetMax = new Vector2(95, 0);
        
            var available = prefab.transform.Find("Available");
            rectTransform.SetParent(available);
        }
    }
}
