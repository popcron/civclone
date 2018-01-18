using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class EditorCivConverter : EditorWindow
{
    [MenuItem("Custom/Converter")]
    public static void Initialize()
    {
        GetWindow(typeof(EditorCivConverter), false, "Converter");
    }

    private void OnGUI()
    {
        string pathToSave = EditorPrefs.GetString("civ5save");
        pathToSave = EditorGUILayout.TextField("Civ 5 Save", pathToSave);
        EditorPrefs.SetString("civ5save", pathToSave);

        if(GUILayout.Button("Convert"))
        {
            var map = MapManager.Convert(pathToSave);

            if (!Directory.Exists(ConfigManager.Root + "/Maps"))
            {
                Directory.CreateDirectory(ConfigManager.Root + "/Maps");
            }

            string json = JsonUtility.ToJson(map, true);
            File.WriteAllText(ConfigManager.Root + "/Maps/" + name + ".txt", json);
        }
    }
}
