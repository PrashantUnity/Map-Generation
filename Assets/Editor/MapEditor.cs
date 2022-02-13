using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(MapGenerator))]
public class MapEditor : Editor
{
    public override void OnInspectorGUI()
    {
        var map = (MapGenerator)target;
        if (DrawDefaultInspector())
        {
            map.GenerateMap();
        }
        if(GUILayout.Button("Generate Map"))
        {
            map.GenerateMap();
        }
    }
}
