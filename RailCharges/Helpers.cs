using System;
using System.Linq;
using RoR2;
using UnityEngine;
using Object = UnityEngine.Object;

namespace RailCharges;

public static class Helpers
{
    public static T GetPrefabFromEntityStateConfiguration<T>(string assetName, string prefabFieldName) where T : Object
    {
        var configuration = UnityEngine.AddressableAssets.Addressables
            .LoadAssetAsync<RoR2.EntityStateConfiguration>(assetName)
            .WaitForCompletion();
                
        var prefab = (T)configuration.serializedFieldsCollection.serializedFields
            .First(sf => sf.fieldName == prefabFieldName).fieldValue
            .objectValue;

        return prefab;
    }

    public static void Exec(Action action, Action after)
    {
        try
        {
            action();
            after();
        }
        catch (Exception ex)
        {
            RailChargesPlugin.Log.LogError(ex);
        }
    }

    public static bool Raycast(CharacterBody characterBody, Ray ray, out RaycastHit hitInfo)
    {
        return Util.CharacterRaycast(characterBody.gameObject, ray, out hitInfo, 1000f,
            (int)LayerIndex.entityPrecise.mask | (int)LayerIndex.world.mask, QueryTriggerInteraction.UseGlobal);
    }
}