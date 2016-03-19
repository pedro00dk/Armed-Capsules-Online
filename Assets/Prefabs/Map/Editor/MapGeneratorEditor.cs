using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(MapGenerator))]
public class MapGeneratorEditor : Editor {

    bool autoGenerate = false;

    public override void OnInspectorGUI() {
        base.OnInspectorGUI();

        MapGenerator map = (MapGenerator) target;

        if (GUILayout.Button("Generate")) {
            map.GenerateAndLoadMapData();
            autoGenerate = false;
        }
        if (GUILayout.Button("Auto Generate")) {
            autoGenerate = true;
        }
        if (autoGenerate) {
            map.GenerateAndLoadMapData();
        }
    }
}
