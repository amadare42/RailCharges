using On.EntityStates.Railgunner.Scope;
using RailCharges.Configuration;

namespace RailCharges.Patches;

public class FasterWindup : IPatch
{
    public static float WindupDuration = 0.1f;
    private PluginConfig config;

    public FasterWindup()
    {
        On.EntityStates.Railgunner.Scope.BaseWindUp.OnEnter += BaseWindUpOnOnEnter;
    }

    private void BaseWindUpOnOnEnter(BaseWindUp.orig_OnEnter orig, EntityStates.Railgunner.Scope.BaseWindUp self)
    {
        if (this.config.WindUpDurationOverride != self.baseDuration)
        {
            self.baseDuration = this.config.WindUpDurationOverride;
            WindupDuration = self.baseDuration;
        }
        orig(self);
    }

    public void Patch(PluginConfig config)
    {
        this.config = config;
    }
}