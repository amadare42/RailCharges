using RoR2;
using RoR2.UI;
using UnityEngine;

namespace RailCharges;

public class ScopeTracker : MonoBehaviour
{
    public static HGTextMeshProUGUI LastTextMesh;
    private HGTextMeshProUGUI textMesh;

    private void Start()
    {
        LastTextMesh = gameObject.transform.Find("Available/charges").GetComponent<HGTextMeshProUGUI>();
        this.textMesh = LastTextMesh;

        UpdateCharges(Plugin.LocalPlayerCharacterMasterController.body.skillLocator.secondary);
        SetReadyStatus(true);
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

    private void OnDestroy()
    {
        if (LastTextMesh == this.textMesh)
        {
            LastTextMesh = null;
        }
    }

    public static void UpdateCharges(GenericSkill skill)
    {
        if (!LastTextMesh)
        {
            Plugin.Log.LogInfo($"{nameof(UpdateCharges)}: no {nameof(LastTextMesh)}");
            return;
        }

        if (skill.maxStock == 1)
        {
            LastTextMesh.text = "";
            return;
        }
        LastTextMesh.text = $"{skill.stock}";
    }

    public static void SetReadyStatus(bool isReady)
    {
        if (!LastTextMesh)
        {
            return;
        }

        LastTextMesh.alpha = isReady ? 0.5f : .1f;
    }

    private void Update()
    {
        if (!LastTextMesh)
        {
            return;
        }
        
        SetReadyStatus(Plugin.LocalPlayerCharacterMasterController.body.skillLocator.primary.CanExecute());
    }
}