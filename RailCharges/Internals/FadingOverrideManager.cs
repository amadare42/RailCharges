using System;
using RoR2;
using UnityEngine;

namespace RailCharges.Internals;

public class FadingOverrideManager
{
    private FadeOverride Override;

    public FadingOverrideManager()
    {
        On.RoR2.CharacterModel.RefreshObstructorsForCamera += CharacterModelOnRefreshObstructorsForCamera;
    }

    private void CharacterModelOnRefreshObstructorsForCamera(On.RoR2.CharacterModel.orig_RefreshObstructorsForCamera orig, CameraRigController camerarigcontroller)
    {
        try
        {
            orig(camerarigcontroller);

            if (this.Override == null)
                return;

            this.Override.Update(Time.deltaTime);
        }
        catch (Exception ex)
        {
            RailChargesPlugin.Log.LogError(ex);
        }
    }


    public void SetOverride(CharacterModel model, float targetOverride, float transitionTime)
    {
        this.Override = new FadeOverride
        {
            Target = targetOverride,
            StartValue = model.fade,
            TransitionTime = transitionTime,
            Model = model
        };
    }

    public void RemoveOverride()
    {
        this.Override = null;
    }


    public class FadeOverride
    {
        public CharacterModel Model { get; set; }
        public float Target { get; set; }
        public float TransitionTime { get; set; }
        public float StartValue { get; set; }
        public float Elapsed { get; set; }

        public void Update(float deltaTime)
        {
            if (!this.Model)
            {
                return;
            }

            this.Elapsed += deltaTime;
            var p = Mathf.Clamp(this.Elapsed, 0, this.TransitionTime);
            this.Model.fade = Mathf.Lerp(this.StartValue, this.Target, p / this.TransitionTime);
        }
    }
}