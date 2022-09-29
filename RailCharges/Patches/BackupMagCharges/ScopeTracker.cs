using RoR2;
using RoR2.UI;
using UnityEngine;

namespace RailCharges.Patches.BackupMagCharges;

public class ScopeTracker : MonoBehaviour
{
    public static HGTextMeshProUGUI LastTextMesh;
    private HGTextMeshProUGUI textMesh;

    private void Start()
    {
        LastTextMesh = this.gameObject.transform.Find("Available/charges")?.GetComponent<HGTextMeshProUGUI>();
        this.textMesh = LastTextMesh;
        if (!LastTextMesh)
        {
            RailChargesPlugin.Log.LogError("Failed to find text mesh");
            return;
        }
        
        if (!CurrentPlayerTracker.CurrentPlayerBody)
        {
            RailChargesPlugin.Log.LogError("Player body not found");
            return;
        }

        UpdateCharges(CurrentPlayerTracker.CurrentPlayerBody.skillLocator.secondary);
        SetReadyStatus(true);
    }
    
    private void Update()
    {
        if (!LastTextMesh || !CurrentPlayerTracker.CurrentPlayerBody)
        {
            return;
        }
        
        SetReadyStatus(CurrentPlayerTracker.CurrentPlayerBody.skillLocator.primary.CanExecute());
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
            RailChargesPlugin.Log.LogInfo($"{nameof(UpdateCharges)}: no {nameof(LastTextMesh)}");
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
}