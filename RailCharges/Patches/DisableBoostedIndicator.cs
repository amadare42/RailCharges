using System;
using RailCharges.Configuration;
using UnityEngine;

namespace RailCharges.Patches;

public class DisableBoostedIndicator : IPatch, IDisposable
{
    public void Patch(PluginConfig config)
    {
        if (!config.DisabledBoostedIndicator)
        {
            return;
        }

        var overlayPrefab = GetOverlayPrefab();

        for (var i = 0; i < overlayPrefab.childCount; i++)
        {
            overlayPrefab.GetChild(i).gameObject.SetActive(false);
        }
        RailChargesPlugin.Log.LogError("Disabled boost UI element.");
    }

    public void Dispose()
    {
        var overlayPrefab = GetOverlayPrefab();

        for (var i = 0; i < overlayPrefab.childCount; i++)
        {
            overlayPrefab.GetChild(i).gameObject.SetActive(true);
        }
    }

    private Transform GetOverlayPrefab()
    {
        return Helpers.GetPrefabFromEntityStateConfiguration<GameObject>(
            "RoR2/DLC1/Railgunner/EntityStates.Railgunner.Reload.Boosted.asset",
            nameof(EntityStates.Railgunner.Reload.Boosted.overlayPrefab)
        ).transform;
    }
}