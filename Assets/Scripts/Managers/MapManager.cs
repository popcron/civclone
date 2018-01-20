using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapManager : MonoBehaviour
{
    [Serializable]
    public class Map
    {
        public string name;
        public string description;
        public int type;
        public int width;
        public int height;
        public int wrap;

        public List<Tile> map;
    }

    [SerializeField]
    private Map map;
    private static int byteIndex = 0;
    private static byte[] data;
    private static MapManager instance;

    public static Map LoadedMap
    {
        get
        {
            if (!instance) instance = FindObjectOfType<MapManager>();

            return instance.map;
        }
    }

    private void Awake()
    {
        instance = this;
    }

    private void OnEnable()
    {
        instance = this;
    }

    private static byte ReadByte()
    {
        byteIndex++;
        return data[byteIndex - 1];
    }

    private static int ReadInt()
    {
        int result = BitConverter.ToInt32(new byte[] { data[byteIndex], data[byteIndex + 1], data[byteIndex + 2], data[byteIndex + 3] }, 0);
        byteIndex += 4;
        return result;
    }

    private static string[] ReadArray(int length)
    {
        char[] text = new char[length];
        for (int i = byteIndex; i < byteIndex + length; i++)
        {
            text[i - byteIndex] = (char)data[i];
        }
        byteIndex += length;

        return new string(text).Split(new char[] { '\x00' }, StringSplitOptions.RemoveEmptyEntries);
    }

    private static string ReadString(int length)
    {
        char[] text = new char[length];
        for (int i = byteIndex; i < byteIndex + length; i++)
        {
            text[i - byteIndex] = (char)data[i];
        }
        byteIndex += length;
        return new string(text).Replace("\x00", "");
    }

    public static void Load(string name)
    {
        if (!instance) instance = FindObjectOfType<MapManager>();

        if (!Directory.Exists(ConfigManager.Root + "/Maps"))
        {
            Directory.CreateDirectory(ConfigManager.Root + "/Maps");
        }
        
        string json = File.ReadAllText(ConfigManager.Root + "/Maps/" + name + ".txt");
        instance.map = JsonUtility.FromJson<Map>(json);
    }

    public static Map Convert(string filePath)
    {
        Map map = new Map();

        data = File.ReadAllBytes(filePath);
        byteIndex = 0;

        map.type = ReadByte();
        map.width = ReadInt();
        map.height = ReadInt();
        byte players = ReadByte(); //?
        map.wrap = ReadInt();

        int terrainTypeLength = ReadInt();
        int firstFeatureLength = ReadInt();
        int secondFeatureLength = ReadInt();
        int resourceTypeLength = ReadInt();

        int somethingStrange = ReadInt();

        int nameLength = ReadInt();
        int descriptionLength = ReadInt();

        string[] terrainTypes = ReadArray(terrainTypeLength);
        string[] firstFeatures = ReadArray(firstFeatureLength);
        string[] secondFeatures = ReadArray(secondFeatureLength);
        string[] resourceTypes = ReadArray(resourceTypeLength);

        map.name = ReadString(nameLength);
        map.description = ReadString(descriptionLength);

        //int string3Length = ReadInt();
        //string string3 = ReadString(string3Length);

        map.map = new List<Tile>();
        for (int y = 0; y < map.height; y++)
        {
            for (int x = 0; x < map.width; x++)
            {
                int terrainIndex = ReadByte();
                int resourceIndex = ReadByte();
                int firstIndex = ReadByte();
                byte riverIndicator = ReadByte();
                Elevation elevation = (Elevation)ReadByte();
                Continent continent = (Continent)ReadByte();
                int secondIndex = ReadByte();

                Tile tile = new Tile
                {
                    x = x,
                    y = y,
                    terrainType = terrainTypes[terrainIndex],
                    resourceType = resourceIndex != 255 ? resourceTypes[resourceIndex] : "",
                    firstFeature = firstIndex != 255 ? firstFeatures[firstIndex] : "",
                    riverIndicator = riverIndicator,
                    elevation = elevation,
                    continent = continent,
                    secondFeature = secondIndex != 255 ? secondFeatures[secondIndex] : "",
                    unknown = ReadByte()
                };

                map.map.Add(tile);
            }
        }

        return map;
    }
}
