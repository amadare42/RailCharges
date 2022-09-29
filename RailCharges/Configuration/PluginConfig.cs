using System;
using BepInEx.Configuration;

namespace RailCharges.Configuration;

public class PluginConfig
{
    public bool DisplayBackupMagazineCharges { get; set; }

    public float ScopeFov { get; set; }

    public float WindUpDurationOverride { get; set; }

    public CrosshairType Crosshair { get; set; }

    public bool RemoveOuterRectangle { get; set; }

    public bool DisabledBoostedIndicator { get; set; }

    public bool AngularCompensation { get; set; }
    

    public static PluginConfig Load(ConfigFile config)
    {
        var pluginConfig = new PluginConfig();

        config.BindCollection("Rail Charges")
            .Bind("Display Charges", "Display number of Backup Magazine charges in scope mode",
                v => pluginConfig.DisplayBackupMagazineCharges = v,
                defaultValue: true
            )
            .Bind("WindUp duration override",
                "Modify how long animation for entering scope mode will take. Default game value is 0.1.",
                v => pluginConfig.WindUpDurationOverride = v,
                defaultValue: .1f
            )
            .Bind("Scope FOV override",
                "Set FOV for scope mode. Lower value - greater zoom. Default game value is 30.",
                v => pluginConfig.ScopeFov = v,
                defaultValue: 30f
            )
            .Bind("Crosshair", new ConfigDescription("Crosshair in normal mode",
                acceptableValues: new AcceptableValueList<string>(Enum.GetNames(typeof(CrosshairType)))),
                set: v => pluginConfig.Crosshair = ParseEnum<CrosshairType>(v),
                defaultValue: CrosshairType.Unchanged.ToString()
            )
            .Bind("Remove Outer Rectangle","Disable outer yellow rectangle in non-scope view.",
                set: v => pluginConfig.RemoveOuterRectangle = v,
                defaultValue: false
            )
            .Bind("Disable Boosted Indicator","Disable boosted indicator for boosted shot after successful reload.",
                set: v => pluginConfig.DisabledBoostedIndicator = v,
                defaultValue: false
            )
            .Bind("Angular Compensation",
                "Compensate camera rotation to retain targeting when entering scope mode. If disabled, scope will be aiming a little bit lower than in normal mode as in original. (Experimental)",
                set: v => pluginConfig.AngularCompensation = v,
                defaultValue: false
            );

        return pluginConfig;
    }

    private static T ParseEnum<T>(string name) where T : struct
    {
        Enum.TryParse(name, out T result);
        return result;
    }
}