using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class EditorAssetsHelper : AssetPostprocessor
{
    static string[] importedAssets;
    static string[] deletedAssets;
    static string[] movedAssets;
    static string[] movedFromAssetPaths;

    static bool Contains(string word)
    {
        return string.Join("\n", importedAssets).Contains(word) || string.Join("\n", deletedAssets).Contains(word) || string.Join("\n", movedAssets).Contains(word) || string.Join("\n", movedFromAssetPaths).Contains(word);
    }

    static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
    {
        EditorAssetsHelper.importedAssets = importedAssets;
        EditorAssetsHelper.deletedAssets = deletedAssets;
        EditorAssetsHelper.movedAssets = movedAssets;
        EditorAssetsHelper.movedFromAssetPaths = movedFromAssetPaths;
        
        if (Contains("Assets/Data/Buildings"))
        {
            EditorHelper.LoadBuildings();
        }
        if (Contains("Assets/Data/Nations"))
        {
            EditorHelper.LoadNations();
        }
        if (Contains("Assets/Data/Units"))
        {
            EditorHelper.LoadUnits();
        }
        if (Contains("Assets/Data/Tasks"))
        {
            EditorHelper.LoadTasks();
        }
        if (Contains("Assets/Data/Improvements"))
        {
            EditorHelper.LoadImprovements();
        }
        if (Contains("Assets/Data/Resources"))
        {
            EditorHelper.LoadResources();
        }
        if (Contains("Assets/Data/Biomes"))
        {
            EditorHelper.LoadBiomes();
        }
    }
}