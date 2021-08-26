using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


[CustomEditor(typeof(MapGenerator))]
public class MapEditor : Editor
{
    // obje seçili olduğu zaman çalışan bir metod
    public override void OnInspectorGUI()
    {
        // yardımcı fonksiyolar sağlanıyor base class tarafından
        base.OnInspectorGUI();

        MapGenerator map = target as MapGenerator;
        map.GenerateMap();
    }
}
