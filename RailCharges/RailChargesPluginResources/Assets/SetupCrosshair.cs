using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CrosshairResources))]
public class SetupCrosshair : MonoBehaviour
{
    public void Clear()
    {
        if (this.transform.childCount == 0) return;
        Transform go;
        while (this.transform.childCount != 0)
        {
            go = this.transform.GetChild(0);
            DestroyImmediate(go.gameObject);
        }
    }

    public void Start()
    {
        Setup();
    }
    
    public void Setup()
    {
        Clear();
        var resources = this.GetComponent<CrosshairResources>();
        var coords = new[]
        {
            new[] { 0, 1, 180 },
            new[] { 1, 0, 90 },
            new[] { 0, -1, 0 },
            new[] { -1, 0, -90 }
        };

        foreach (var coord in coords)
        {
            var x = coord[0];
            var y = coord[1];
            var angle = coord[2];

            var go = new GameObject($"Reticle_{x}_{y}");
            var image = go.AddComponent<Image>();
            image.sprite = resources.reticle;
            image.color = new Color(1, 1, 1, resources.alpha);
            var rectTransform = image.rectTransform;
            
            rectTransform.SetParent(this.transform);
            rectTransform.position = Vector3.zero;
            rectTransform.anchoredPosition = new Vector2(
                resources.shift * x,
                resources.shift * y
            );
            rectTransform.sizeDelta = new Vector2(resources.size, resources.size);
            rectTransform.rotation = Quaternion.Euler(0, 0, angle);
        }

        var centerGo = new GameObject("center");
        var img = centerGo.AddComponent<Image>();
        img.color = new Color(1, 1, 1, resources.alpha);
        img.sprite = resources.center;
        centerGo.transform.SetParent(this.transform);
        img.rectTransform.anchoredPosition = Vector2.zero;
        img.rectTransform.sizeDelta = new Vector2(resources.size, resources.size);
    }
}
