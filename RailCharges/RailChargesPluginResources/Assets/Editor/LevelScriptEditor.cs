using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(SetupCrosshair))]
public class LevelScriptEditor : Editor 
{
    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        var myTarget = (SetupCrosshair)this.target;

        if (GUILayout.Button("Setup"))
        {
            myTarget.Setup();
        }
        
        if (GUILayout.Button("Clear"))
        {
            myTarget.Clear();
        }
    }
}