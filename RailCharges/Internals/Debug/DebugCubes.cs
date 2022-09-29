using System.Collections.Generic;
using UnityEngine;

namespace RailCharges.Internals.Debug;

public class DebugCubes
{
    private static Dictionary<Color, GameObject> cubes = new();

    public static void Clear()
    {
        foreach (var cube in cubes.Values)
        {
            Object.Destroy(cube);
        }
        cubes.Clear();
    }

    public static void SetPosition(Color color, Vector3 position, float size = .1f)
    {
        GetOrCreateCube(color, size).transform.position = position;
    }

    public static GameObject GetOrCreateCube(Color color, float size = .1f)
    {
        if (cubes.TryGetValue(color, out var cube) && cube)
        {
            return cube;
        }
        else
        {
            cube = CreateDebugCube($"debug_cube_{color}", color, size);
            cubes[color] = cube;
        }

        return cube;
    }
    
    private static GameObject CreateDebugCube(string name, Color color, float size = 0.1f)
    {
        var cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        
        cube.name = name;
        cube.GetComponent<Renderer>().material.color = color;
        cube.layer = LayerMask.NameToLayer("Ignore Raycast");
        cube.transform.localScale = new Vector3(size,size,size);

        return cube;
    }
}