using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(GroundGenerator))]
class GroundGenEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.DrawDefaultInspector();

        GroundGenerator generator = (GroundGenerator)target;
        if (GUILayout.Button("Apply Variables"))
            generator.ApplyVars();
            
    }
}
