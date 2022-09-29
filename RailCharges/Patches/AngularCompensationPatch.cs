using System;
using EntityStates.Railgunner.Scope;
using RailCharges.Configuration;
using RailCharges.Internals;
using RailCharges.Internals.Debug;
using RoR2;
using UnityEngine;
using BaseScopeState = On.EntityStates.Railgunner.Scope.BaseScopeState;

namespace RailCharges.Patches;

public class AngularCompensationPatch : IPatch, IDisposable
{
    private readonly CameraNudgeManager cameraNudgeManager;
    private PluginConfig config;

    public AngularCompensationPatch(CameraNudgeManager cameraNudgeManager)
    {
        this.cameraNudgeManager = cameraNudgeManager;
        
        On.EntityStates.Railgunner.Scope.BaseScopeState.StartScopeParamsOverride += OnBaseScopeStateOnStartScopeParamsOverride;
        On.EntityStates.Railgunner.Scope.BaseScopeState.EndScopeParamsOverride += (orig, self, transitionDuration) => OnBaseScopeStateOnEndScopeParamsOverride(orig, self, transitionDuration);
    }
    
    private void OnBaseScopeStateOnStartScopeParamsOverride(On.EntityStates.Railgunner.Scope.BaseScopeState.orig_StartScopeParamsOverride orig, EntityStates.Railgunner.Scope.BaseScopeState self, float transitionDuration)
    {
        Helpers.Exec(() => orig(self, transitionDuration), () =>
        {
            if (!this.config.AngularCompensation) return;
            if (self is WindUpScopeHeavy && self.characterBody.master.playerCharacterMasterController.networkUser.isLocalPlayer)
            {
                OnEnteringScope(self.characterBody);
            }
        });
    }

    private void OnBaseScopeStateOnEndScopeParamsOverride(BaseScopeState.orig_EndScopeParamsOverride orig,
        EntityStates.Railgunner.Scope.BaseScopeState self, float transitionDuration)
    {
        Helpers.Exec(() => orig(self, transitionDuration), () =>
        {
            if (!this.config.AngularCompensation) return;
            if (self is BaseWindDown && self.characterBody.master.playerCharacterMasterController.networkUser.isLocalPlayer)
            {
                OnExitingScope(self.characterBody);
            }
        });
    }

    public void Patch(PluginConfig config)
    {
        this.config = config;
    }

    public static CameraRigController CameraRigController => Camera.main.gameObject.transform.parent.GetComponent<CameraRigController>();

    private void OnEnteringScope(CharacterBody body)
    {
        this.cameraNudgeManager.Cancel();
        RailChargesPlugin.Log.LogInfo("Entering scope");
        var cameraTransform = Camera.main.transform;
        var ray = new Ray(cameraTransform.position, cameraTransform.forward);
        if (Helpers.Raycast(body, ray, out RaycastHit hitInfo))
        {
            RailChargesPlugin.Log.LogInfo("Raycast hit " + hitInfo.collider.gameObject.name);
            
            var scopePosition = CameraRigController.target.transform.position + new Vector3(0, 0.5f, 0);

#if DEBUG
            DebugCubes.SetPosition(Color.green, hitInfo.point);
            DebugCubes.SetPosition(Color.red, cameraTransform.position);
            DebugCubes.SetPosition(Color.blue, scopePosition);
#endif
            
            var lookVector = hitInfo.point - scopePosition;
            var targetRot = Quaternion.LookRotation(lookVector);

            RailChargesPlugin.Log.LogInfo(targetRot.eulerAngles);
            this.cameraNudgeManager.Set(new Vector2(0, AngleDifference(targetRot.eulerAngles.x, cameraTransform.rotation.eulerAngles.x)), FasterWindup.WindupDuration);
        }
    }
    
    private void OnExitingScope(CharacterBody characterBody)
    {
        this.cameraNudgeManager.Cancel();
        RailChargesPlugin.Log.LogInfo("Exiting scope");
        var cameraTransform = Camera.main.transform;
        var basic = CharacterCameraParamsData.basic;
        var ray = new Ray(cameraTransform.position, cameraTransform.forward);
        if (Helpers.Raycast(characterBody, ray, out RaycastHit hitInfo))
        {
            var noScopePosition = cameraTransform.position 
                      + cameraTransform.rotation * basic.idealLocalCameraPos.value
                      + new Vector3(0, basic.pivotVerticalOffset.value, 0);
            
            var r = hitInfo.point - noScopePosition;
            var targetRot = Quaternion.LookRotation(r);
            this.cameraNudgeManager.Set(new Vector2(0, AngleDifference(targetRot.eulerAngles.x, cameraTransform.rotation.eulerAngles.x)), .01f);
        }
    }
    
    public static float AngleDifference( float angle1, float angle2 )
    {
        float diff = ( angle2 - angle1 + 180 ) % 360 - 180;
        return diff < -180 ? diff + 360 : diff;
    }

    public void Dispose()
    {
        On.EntityStates.Railgunner.Scope.BaseScopeState.StartScopeParamsOverride -= OnBaseScopeStateOnStartScopeParamsOverride;
        On.EntityStates.Railgunner.Scope.BaseScopeState.EndScopeParamsOverride -= (orig, self, transitionDuration) => OnBaseScopeStateOnEndScopeParamsOverride(orig, self, transitionDuration);
    }
}