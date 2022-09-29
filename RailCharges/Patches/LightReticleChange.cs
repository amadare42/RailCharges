using System.Collections.Generic;
using RailCharges.Configuration;
using RailCharges.Resources;
using RoR2.UI;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace RailCharges.Patches;

public class LightReticleChange : IPatch
{
    public void Patch(PluginConfig config)
    {
        if (config.Crosshair == CrosshairType.Unchanged && !config.RemoveOuterRectangle)
        {
            return;
        }

        var prefab = UnityEngine.AddressableAssets.Addressables
            .LoadAssetAsync<GameObject>("RoR2/DLC1/Railgunner/RailgunnerCrosshair.prefab")
            .WaitForCompletion();

        var scaler = prefab.transform.Find("Scaler");
        if (!scaler)
        {
            RailChargesPlugin.Log.LogError("Cannot find Scaler - light scope wasn't patched!");
            return;
        }
        var controller = prefab.GetComponent<CrosshairController>();
        var scalerRect = (RectTransform)scaler;

        if (config.RemoveOuterRectangle)
        {
            RemoveOuterRectangle(scalerRect);
        }

        switch (config.Crosshair)
        {
            case CrosshairType.X:
                CreateXCrosshair(scalerRect, controller);
                break;

            case CrosshairType.SameAsScope:
                CreatePlusCrosshair(scalerRect, controller);
                break;

            case CrosshairType.Remove:
                CreateNoRectangleCrosshair(scalerRect, controller);
                break;

            case CrosshairType.Unchanged:
                return;
        }

        RailChargesPlugin.Log.LogInfo($"Scope reticles patched!");
    }

    private static void RemoveOuterRectangle(RectTransform scaler)
    {
        var flavor = scaler.parent.Find("Flavor, Ready");
        if (!flavor)
        {
            RailChargesPlugin.Log.LogWarning($"Cannot find outer rectangle to remove!");
            return;
        }
        for (var i = 0; i < flavor.childCount; i++)
        {
            flavor.GetChild(i).gameObject.SetActive(false);
        }
        RailChargesPlugin.Log.LogInfo($"Outer rectangle removed");
    }

    public static void CreatePlusCrosshair(RectTransform scaler, CrosshairController controller)
    {
        DestroyExistingCrosshair(scaler);
        var heavyPrefab = UnityEngine.AddressableAssets.Addressables
            .LoadAssetAsync<GameObject>("RoR2/DLC1/Railgunner/RailgunnerScopeCrosshairHeavy.prefab")
            .WaitForCompletion();
        var available = heavyPrefab.transform.Find("Available");
        var clone = Object.Instantiate(available);
        clone.transform.SetParent(scaler);

        var reticles = new List<GameObject>(4);
        for (var i = 0; i < clone.childCount; i++)
        {
            var go = clone.GetChild(i).gameObject;
            if (go.name == "Image")
            {
                reticles.Add(go);
            }
        }
        
        const int zeroOffset = -15;
        const int shotOffset = 50;

        var lst = new List<CrosshairController.SpritePosition>();

        var quads = new[]
        {
            new[] { 1, 0 },
            new[] { -1, 0 },
            new[] { 0, 1 },
            new[] { 0, -1 },
        };
        for (var index = 0; index < quads.Length; index++)
        {
            var x = quads[index][0];
            var y = quads[index][1];
            var reticle = reticles[index];
            
            lst.Add(new CrosshairController.SpritePosition
            {
                target = (RectTransform)reticle.transform,
                zeroPosition = reticle.transform.position + new Vector3(zeroOffset * x, zeroOffset * y),
                onePosition = reticle.transform.position + new Vector3(shotOffset * x, shotOffset * y)
            });
            reticle.transform.SetParent(scaler);
        }

        controller.spriteSpreadPositions = lst.ToArray();
    }

    private static void CreateXCrosshair(RectTransform scaler, CrosshairController controller)
    {
        DestroyExistingCrosshair(scaler);
        CreateCustomReticles(scaler, controller);
    }

    private static void DestroyExistingCrosshair(RectTransform scaler)
    {
        for (var i = 0; i < scaler.childCount; i++)
        {
            Object.Destroy(scaler.GetChild(i).gameObject);
        }
    }

    private static void CreateNoRectangleCrosshair(RectTransform scaler, CrosshairController controller)
    {
        for (var i = 0; i < scaler.childCount; i++)
        {
            scaler.GetChild(i).gameObject.GetComponent<Image>().color = Color.clear;
        }
    }

    public static void CreateCustomReticles(RectTransform scaler, CrosshairController controller)
    {
        const int offset = 15;
        const int shotOffset = 50;

        var lst = new List<CrosshairController.SpritePosition>();
        
        int[] quads = { -1, 1 };
        foreach (var x in quads)
        foreach (var y in quads)
        {
            var reticle = CreateReticle(offset * x, offset * y);
            lst.Add(new CrosshairController.SpritePosition
            {
                target = (RectTransform)reticle.transform,
                zeroPosition = new Vector3(offset * x, offset * y),
                onePosition = new Vector3(shotOffset * x, shotOffset * y)
            });
            reticle.transform.SetParent(scaler);
        }

        controller.spriteSpreadPositions = lst.ToArray();
    }

    private static GameObject CreateReticle(int x, int y)
    {
        var go = new GameObject($"Reticle_{x}_{y}");

        var imageComponent = go.AddComponent<Image>();
        imageComponent.sprite = ResourceManager.Get<Sprite>("reticle");
        imageComponent.color = new Color(1f, 1f, 1f, .5f);

        var transform = (RectTransform)go.transform;
        transform.anchoredPosition = new Vector2(x, y);
        transform.sizeDelta = new Vector2(15, 15);
        if (x != y)
        {
            transform.localRotation = Quaternion.Euler(0, 180, 0);
        }

        return go;
    }
}