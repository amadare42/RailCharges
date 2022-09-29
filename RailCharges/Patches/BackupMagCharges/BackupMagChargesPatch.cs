using System;
using EntityStates.Railgunner.Scope;
using RailCharges.Configuration;
using RoR2.UI;
using UnityEngine;
using Object = UnityEngine.Object;

namespace RailCharges.Patches.BackupMagCharges;

using static Helpers;

public class BackupMagChargesPatch : IPatch
{
    public void Patch(PluginConfig config)
    {
        if (!config.DisplayBackupMagazineCharges)
        {
            return;
        }

        On.RoR2.CharacterBody.OnSkillActivated += (orig, self, skill) =>
        {
            try
            {
                orig(self, skill);

                if (self.hasEffectiveAuthority && skill.skillNameToken is "RAILGUNNER_SNIPE_HEAVY_NAME"  && self.master.playerCharacterMasterController.networkUser.isLocalPlayer)
                {
                    ScopeTracker.UpdateCharges(self.skillLocator.primary);
                }
            }
            catch (Exception ex)
            {
                RailChargesPlugin.Log.LogError(ex);
            }
        };

        var heavyScopeCrosshairPrefab = GetPrefabFromEntityStateConfiguration<GameObject>(
            "RoR2/DLC1/Railgunner/EntityStates.Railgunner.Scope.ActiveScopeHeavy.asset",
            nameof(ActiveScopeHeavy.crosshairOverridePrefab)
        );
        PatchPrefab(heavyScopeCrosshairPrefab);
        RailChargesPlugin.Log.LogInfo($"Heavy crosshair prefab patched!");
    }
    
    public static void PatchPrefab(GameObject prefab)
    {
        // NOTE: using string type instead of type to facilitate live-reload
        var existing = prefab.GetComponent(typeof(ScopeTracker).FullName);
        if (existing) Object.Destroy(existing);
        prefab.AddComponent<ScopeTracker>();

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