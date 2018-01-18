using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct HexCoordinate
{
    [SerializeField]
    private int x, z;

    public int X
    {
        get
        {
            return x;
        }
    }

    public int Z
    {
        get
        {
            return z;
        }
    }

    public int Y
    {
        get
        {
            return -X - Z;
        }
    }

    public static HexCoordinate FromOffsetCoordinates(int x, int z)
    {
        return new HexCoordinate(x - z / 2, z);
    }

    public static HexCoordinate FromPosition(Vector3 position)
    {
        float x = position.x / (WorldManager.InnerRadius * 2f);
        float y = -x;

        float offset = position.z / (WorldManager.OuterRadius * 3f);
        x -= offset;
        y -= offset;

        int iX = Mathf.RoundToInt(x);
        int iY = Mathf.RoundToInt(y);
        int iZ = Mathf.RoundToInt(-x - y);

        if (iX + iY + iZ != 0)
        {
            float dX = Mathf.Abs(x - iX);
            float dY = Mathf.Abs(y - iY);
            float dZ = Mathf.Abs(-x - y - iZ);

            if (dX > dY && dX > dZ)
            {
                iX = -iY - iZ;
            }
            else if (dZ > dY)
            {
                iZ = -iX - iY;
            }
        }

        return new HexCoordinate(iX, iZ);
    }

    public override string ToString()
    {
        return "(" + X.ToString() + ", " + Y.ToString() + ", " + Z.ToString() + ")";
    }

    public override int GetHashCode()
    {
        unchecked
        {
            int hash = 17;
            hash = hash * 23 + x.GetHashCode();
            hash = hash * 23 + z.GetHashCode();
            return hash;
        }
    }

    public static bool operator ==(HexCoordinate a, HexCoordinate b)
    {
        return a.Equals(b);
    }

    public static bool operator !=(HexCoordinate a, HexCoordinate b)
    {
        return !a.Equals(b);
    }

    public override bool Equals(object obj)
    {
        if(obj is HexCoordinate)
        {
            HexCoordinate coordinate = (HexCoordinate)obj;
            return coordinate.x == x && coordinate.z == z;
        }

        return false;
    }

    public HexCoordinate(int x, int z)
    {
        this.x = x;
        this.z = z;
    }
}

public class WorldManager : MonoBehaviour {

    public static WorldManager instance;
    private WorldTile[] tiles;
    private Transform tilesRoot;

    public List<UnitObject> units = new List<UnitObject>();
    public List<ImprovementObject> improvements = new List<ImprovementObject>();
    public List<ImprovementObjectCity> cities = new List<ImprovementObjectCity>();
    public List<ImprovementObjectCapital> capitals = new List<ImprovementObjectCapital>();

    public WorldRenderer worldRenderer;
    public float tileSize = 1f;

    public static float OuterRadius
    {
        get
        {
            if (!instance) instance = FindObjectOfType<WorldManager>();
            return instance.tileSize;
        }
    }

    public static float InnerRadius
    {
        get
        {
            if (!instance) instance = FindObjectOfType<WorldManager>();
            return instance.tileSize * 0.86602540378f;
        }
    }

    public static WorldTile[] Tiles
    {
        get
        {
            if (!instance) instance = FindObjectOfType<WorldManager>();
            return instance.tiles;
        }
    }

    private void Awake()
    {
        instance = this;
        tilesRoot = worldRenderer.transform.Find("Tiles");
    }

    private void OnEnable()
    {
        instance = this;
    }

    public static WorldTile Get(Vector3 position)
    {
        HexCoordinate coordinate = HexCoordinate.FromPosition(position);
        for(int i = 0; i < instance.tiles.Length;i++)
        {
            if(instance.tiles[i].coordinate == coordinate)
            {
                return instance.tiles[i];
            }
        }

        return null;
    }

    public static WorldTile Get(int x, int z)
    {
        for (int i = 0; i < instance.tiles.Length; i++)
        {
            if (instance.tiles[i].coordinate.X == x && instance.tiles[i].coordinate.Z == z)
            {
                return instance.tiles[i];
            }
        }

        return null;
    }

    public static void Generate(MapManager.Map save)
    {
        if (!instance) instance = FindObjectOfType<WorldManager>();
        instance.tiles = new WorldTile[save.width * save.height];

        Debug.Log(save.width + ", " + save.height);
        Debug.Log(instance.tiles.Length + ", " + save.map.Count);

        for (int i = 0; i < save.map.Count; i++)
        {
            instance.tiles[i] = GenerateTile(save.map[i]);
        }
        for (int i = 0; i < instance.tiles.Length; i++)
        {
            instance.tiles[i].RefreshNeighbors();
        }

        Refresh();

        WorldTile tile = Tiles[Random.Range(0, Tiles.Length)];
        var settler = GameManager.PlaceUnit("Settler", tile, Config.GameSave.nation);
        GameManager.PlaceUnit("Warrior", tile.RandomNeighbour, Config.GameSave.nation);

        Config.Temporary.selectedTile = tile;
        Config.Temporary.selectedUnit = settler;
    }

    public static void Refresh()
    {
        instance.worldRenderer.Triangulate(instance.tiles);
    }

    private static WorldTile GenerateTile(MapManager.Tile tile)
    {
        int x = tile.x;
        int z = tile.y;
        float e = 0f;

        if (tile.elevation == MapManager.Elevation.Flat) e = 0.5f;
        else if (tile.elevation == MapManager.Elevation.Hill) e = 0.6f;
        else if (tile.elevation == MapManager.Elevation.Mountain) e = 1f;
        if (tile.terrainType == "TERRAIN_OCEAN") e = 0f;
        if (tile.terrainType == "TERRAIN_COAST") e = 0.1f;

        Vector3 position = new Vector3
        {
            x = (x + z * 0.5f - z / 2) * InnerRadius * 2f,
            y = e,
            z = z * OuterRadius * 1.5f
        };

        float a = (float)x / Config.GameSave.width;
        float b = (float)z / Config.GameSave.height;
        float m = Mathf.PerlinNoise(a + 1, b + 1);

        WorldTile worldTile = new GameObject("Tile").AddComponent<WorldTile>();
        worldTile.transform.SetParent(instance.tilesRoot);
        worldTile.transform.localPosition = position;
        worldTile.coordinate = HexCoordinate.FromOffsetCoordinates(x, z);

        if (tile.terrainType == "TERRAIN_OCEAN") worldTile.color = Color.blue;
        if (tile.terrainType == "TERRAIN_SNOW") worldTile.color = Color.white;
        if (tile.terrainType == "TERRAIN_COAST") worldTile.color = Color.cyan;
        if (tile.terrainType == "TERRAIN_GRASS") worldTile.color = Color.green;
        if (tile.terrainType == "TERRAIN_PLAINS") worldTile.color = Color.green;
        if (tile.terrainType == "TERRAIN_DESERT") worldTile.color = Color.yellow;
        if (tile.terrainType == "TERRAIN_TUNDRA") worldTile.color = Color.gray;
        if (tile.elevation == MapManager.Elevation.Mountain) worldTile.color *= 0.5f;

        return worldTile;
    }
}
