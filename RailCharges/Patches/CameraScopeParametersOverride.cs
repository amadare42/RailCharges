using System;
using EntityStates.Railgunner.Scope;
using HG.BlendableTypes;
using RailCharges.Configuration;
using RoR2;

namespace RailCharges.Patches;

public class CameraScopeParametersOverride : IPatch
{
    public void Patch(PluginConfig config)
    {
        OverrideScopeCameraParameters(config);
    }

    private void OverrideScopeCameraParameters(PluginConfig config)
    {
        var cameraParams = Helpers.GetPrefabFromEntityStateConfiguration<CharacterCameraParams>(
            "RoR2/DLC1/Railgunner/EntityStates.Railgunner.Scope.WindUpScopeHeavy.asset",
            nameof(WindUpScopeHeavy.cameraParams)
        );
        cameraParams.data.fov = new BlendableFloat
        {
            value = config.ScopeFov,
            alpha = 1
        };
    }

}