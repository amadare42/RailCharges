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
        LastTextMesh = gameObject.transform.Find("Available/charges")?.GetComponent<HGTextMeshProUGUI>();
        this.textMesh = LastTextMesh;
        if (!LastTextMesh)
        {
            Plugin.Log.LogError("Failed to find text mesh");
            return;
        }
        
        if (!Plugin.LocalPlayerBody) 
        {
            Plugin.Log.LogError("Player body not found");
            return;
        }

        UpdateCharges(Plugin.LocalPlayerBody.skillLocator.secondary);
        SetReadyStatus(true);
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
        
        SetReadyStatus(Plugin.LocalPlayerBody.skillLocator.primary.CanExecute());
    }
}