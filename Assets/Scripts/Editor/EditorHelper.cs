using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class EditorHelper : EditorWindow
{
    [MenuItem("Custom/Helper")]
    public static void Initialize()
    {
        GetWindow(typeof(EditorHelper), false, "Helper");
    }

    private void OnGUI()
    {
        if(GUILayout.Button("Refresh Canvas"))
        {
            CanvasManager.Refresh();
        }
        if (GUILayout.Button("Refresh World"))
        {
            WorldManager.Refresh();
        }

        for (int i = 0; i < CanvasManager.Layers.Length;i++)
        {
            if(GUILayout.Button(CanvasManager.Layers[i].name))
            {
                CanvasManager.Layer = CanvasManager.Layers[i].name;
            }
        }
    }

    public static void LoadBuildings()
    {
        GameManager gameManager = GameObject.FindObjectOfType<GameManager>();
        if (gameManager)
        {
            gameManager.buildings.Clear();
            string[] guids = AssetDatabase.FindAssets("t:Building");
            for (int i = 0; i < guids.Length; i++)
            {
                string path = AssetDatabase.GUIDToAssetPath(guids[i]);
                Building asset = AssetDatabase.LoadAssetAtPath<Building>(path);
                gameManager.buildings.Add(asset);
            }
        }
    }

    public static void LoadImprovements()
    {
        GameManager gameManager = GameObject.FindObjectOfType<GameManager>();
        if (gameManager)
        {
            gameManager.improvements.Clear();
            string[] guids = AssetDatabase.FindAssets("t:Improvement");
            for (int i = 0; i < guids.Length; i++)
            {
                string path = AssetDatabase.GUIDToAssetPath(guids[i]);
                Improvement asset = AssetDatabase.LoadAssetAtPath<Improvement>(path);
                gameManager.improvements.Add(asset);
            }
        }
    }

    public static void LoadNations()
    {
        GameManager gameManager = GameObject.FindObjectOfType<GameManager>();
        if (gameManager)
        {
            gameManager.nations.Clear();
            string[] guids = AssetDatabase.FindAssets("t:Nation");
            for (int i = 0; i < guids.Length; i++)
            {
                string path = AssetDatabase.GUIDToAssetPath(guids[i]);
                Nation asset = AssetDatabase.LoadAssetAtPath<Nation>(path);
                gameManager.nations.Add(asset);
            }
        }
    }

    public static void LoadUnits()
    {
        GameManager gameManager = GameObject.FindObjectOfType<GameManager>();
        if (gameManager)
        {
            gameManager.units.Clear();
            string[] guids = AssetDatabase.FindAssets("t:Unit");
            for (int i = 0; i < guids.Length; i++)
            {
                string path = AssetDatabase.GUIDToAssetPath(guids[i]);
                Unit asset = AssetDatabase.LoadAssetAtPath<Unit>(path);
                gameManager.units.Add(asset);
            }
        }
    }

    public static void LoadBiomes()
    {
        GameManager gameManager = GameObject.FindObjectOfType<GameManager>();
        if (gameManager)
        {
            gameManager.biomes.Clear();
            string[] guids = AssetDatabase.FindAssets("t:Biome");
            for (int i = 0; i < guids.Length; i++)
            {
                string path = AssetDatabase.GUIDToAssetPath(guids[i]);
                Biome asset = AssetDatabase.LoadAssetAtPath<Biome>(path);
                gameManager.biomes.Add(asset);
            }
        }
    }

    public static void LoadResources()
    {
        GameManager gameManager = GameObject.FindObjectOfType<GameManager>();
        if (gameManager)
        {
            gameManager.resources.Clear();
            string[] guids = AssetDatabase.FindAssets("t:Resource");
            for (int i = 0; i < guids.Length; i++)
            {
                string path = AssetDatabase.GUIDToAssetPath(guids[i]);
                Resource asset = AssetDatabase.LoadAssetAtPath<Resource>(path);
                gameManager.resources.Add(asset);
            }
        }
    }

    public static void LoadTasks()
    {
        GameManager gameManager = GameObject.FindObjectOfType<GameManager>();
        if (gameManager)
        {
            gameManager.tasks.Clear();
            string[] guids = AssetDatabase.FindAssets("t:Task");
            for (int i = 0; i < guids.Length; i++)
            {
                string path = AssetDatabase.GUIDToAssetPath(guids[i]);
                Task asset = AssetDatabase.LoadAssetAtPath<Task>(path);
                gameManager.tasks.Add(asset);
            }
        }
    }
}
