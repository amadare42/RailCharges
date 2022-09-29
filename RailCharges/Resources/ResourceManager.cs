
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using Object = UnityEngine.Object;

namespace RailCharges.Resources;

public class ResourceManager
{
    private static Dictionary<string, Object> dict = new();

    public static void Init()
    {
        using var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("RailCharges.Resources.railcharges");
        var bundle = AssetBundle.LoadFromStream(stream);
        dict["reticle"] = bundle.LoadAsset<Sprite>("Assets/icons/reticle.png");
        bundle.Unload(false);
    }

    public static T Get<T>(string name) where T : Object
    {
        if (dict.TryGetValue(name, out var value))
        {
            return (T)value;
        }

        return null;
    }
}