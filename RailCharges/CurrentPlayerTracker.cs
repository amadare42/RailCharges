using System.Linq;
using RoR2;

namespace RailCharges;

public class CurrentPlayerTracker
{
    private static PlayerCharacterMasterController cachedCurrentMaster;

    public static void Init()
    {
    }

    public static CharacterBody CurrentPlayerBody
    {
        get
        {
            var characterBody = cachedCurrentMaster?.master.GetBody();
            if (characterBody)
            {
                return characterBody;
            }

            cachedCurrentMaster = PlayerCharacterMasterController.instances.FirstOrDefault(c => c.networkUser.isLocalPlayer);
            characterBody = cachedCurrentMaster?.master.GetBody();
            if (!characterBody)
            {
                RailChargesPlugin.Log.LogInfo("Local player body found!");
            }

            return characterBody;
        }        
    }
}