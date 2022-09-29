using UnityEngine;

namespace RailCharges.Internals;

public class CameraNudgeManager
{
    private Nudge nudge;

    public CameraNudgeManager()
    {
        On.RoR2.CameraModes.CameraModeBase.CollectLookInput += CameraModeBaseOnCollectLookInput;
    }

    private void CameraModeBaseOnCollectLookInput(On.RoR2.CameraModes.CameraModeBase.orig_CollectLookInput orig, RoR2.CameraModes.CameraModeBase self, ref RoR2.CameraModes.CameraModeBase.CameraModeContext context, out RoR2.CameraModes.CameraModeBase.CollectLookInputResult result)
    {
        orig(self, ref context, out result);
        if (this.nudge == null) return;
        result.lookInput += this.nudge.Tick(Time.deltaTime);
        if (this.nudge.IsDone)
        {
            RailChargesPlugin.Log.LogInfo($"Nudge finished @{this.nudge.Elapsed}");
            Cancel();
        }
    }

    public void Set(Vector2 delta, float duration)
    {
        this.nudge = new Nudge
        {
            Delta = delta,
            Elapsed = 0,
            Progress = Vector2.zero,
            Duration = duration
        };
        RailChargesPlugin.Log.LogInfo($"Nudge was set {delta} {duration:00.00}");
    }

    public void Cancel()
    {
        this.nudge = null;
    }

    public class Nudge
    {
        public Vector2 Delta { get; set; }
        public Vector2 Progress { get; set; }
        public float Elapsed { get; set; }
        public float Duration { get; set; }

        public bool IsDone => this.Elapsed >= this.Duration;


        public Vector2 Tick(float deltaTime)
        {
            // calculate progress
            this.Elapsed += deltaTime;
            var progress = Mathf.Clamp01(this.Elapsed / this.Duration);

            var lerpResult = Vector2.Lerp(Vector2.zero, this.Delta, progress);
            var frameDelta = lerpResult - this.Progress;
            this.Progress = lerpResult;

            return frameDelta;
        }
    }
}