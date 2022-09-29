using On.RoR2.CharacterAI;

namespace RailCharges.Internals.Debug;

public class DisableAI
{
    public static bool Disabled { get; set; }

    public static void Init()
    {
        On.RoR2.CharacterAI.BaseAI.FixedUpdate += BaseAIOnFixedUpdate;
    }

    private static void BaseAIOnFixedUpdate(BaseAI.orig_FixedUpdate orig, RoR2.CharacterAI.BaseAI self)
    {
        if (Disabled) return;
        orig(self);
    }
}