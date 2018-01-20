using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

public interface ITurnWaiter
{
    void NewTurn();
}

public class GameManager : MonoBehaviour
{
    public GameObject unitPrefab;

    public List<Unit> units = new List<Unit>();
    public List<Nation> nations = new List<Nation>();
    public List<Building> buildings = new List<Building>();
    public List<Improvement> improvements = new List<Improvement>();
    public List<Resource> resources = new List<Resource>();
    public List<Biome> biomes = new List<Biome>();
    public List<Task> tasks = new List<Task>();

    public static GameManager instance;

    private TileObject lastTile;
    private List<TileObject> path;

    public static Vector3 MousePosition
    {
        get; private set;
    }

    public static List<Unit> Units
    {
        get
        {
            if (!instance) instance = FindObjectOfType<GameManager>();

            return instance.units;
        }
    }

    public static List<Building> Buildings
    {
        get
        {
            if (!instance) instance = FindObjectOfType<GameManager>();

            return instance.buildings;
        }
    }

    public static List<Improvement> Improvements
    {
        get
        {
            if (!instance) instance = FindObjectOfType<GameManager>();

            return instance.improvements;
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

    private void Update()
    {
        if (!EventSystem.current.IsPointerOverGameObject())
        {
            HandleInput();
        }

        if(Config.Temporary.selectedTile)
        {
            Gizmos.DrawTile(Config.Temporary.selectedTile.transform.position + Vector3.up * 0.05f, WorldManager.OuterRadius, Color.red);
        }
        if (Config.Temporary.selectedImprovement)
        {
            Gizmos.DrawTile(Config.Temporary.selectedImprovement.transform.position + Vector3.up * 0.05f, WorldManager.InnerRadius, Color.cyan);
        }
        if (Config.Temporary.selectedUnit)
        {
            Gizmos.DrawTile(Config.Temporary.selectedUnit.transform.position + Vector3.up * 0.05f, WorldManager.InnerRadius, Color.blue);
        }

        ProcessLocalNation();
    }

    private void ProcessLocalNation()
    {
        Nation nation = Config.GameSave.nation;
        if(nation)
        {
            Config.GameSave.sciencePerTurn = nation.GetStatistic("Science");
            Config.GameSave.goldPerTurn = nation.GetStatistic("Gold");
            Config.GameSave.faithPerTurn = nation.GetStatistic("Faith");
            Config.GameSave.culturePerTurn = nation.GetStatistic("Culture");
        }
    }

    private void HandleInput()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit = new RaycastHit();
        TileObject tile = null;
        if (Physics.Raycast(ray, out hit))
        {
            MousePosition = hit.point + Camera.main.transform.forward * 0.1f;

            HexCoordinate coordinates = HexCoordinate.FromPosition(MousePosition);
            int index = coordinates.X + coordinates.Z * Config.GameSave.width + coordinates.Z / 2;
            if (WorldManager.Tiles.Length > index && index >= 0)
            {
                tile = WorldManager.Tiles[index];
            }
            else
            {
                tile = null;
            }
        }
        else
        {
            MousePosition = Vector3.zero;
            tile = null;
        }
        
        if (Input.GetMouseButtonUp(0) && !CameraManager.Dragging)
        {
            Config.Temporary.selectedTile = tile;
            CanvasLayerInGame.RefreshSelectedTile();
        }
        if(Config.Temporary.selectedUnit)
        {
            if(Input.GetMouseButtonDown(1))
            {
                path = WorldPathfinding.FindPath(Config.Temporary.selectedTile, tile).ToList();
            }

            if (Input.GetMouseButton(1))
            {
                if(tile)
                {
                    Gizmos.DrawTile(tile.transform.position + Vector3.up * 0.05f, WorldManager.InnerRadius, Color.white);
                }

                if(lastTile != tile && tile)
                {
                    var newPath = WorldPathfinding.FindPath(Config.Temporary.selectedTile, tile);
                    if(newPath != null)
                    {
                        path = newPath.ToList();
                    }
                }

                if(path != null && tile)
                {
                    for(int i = 0; i < path.Count;i++)
                    {
                        float size = (Mathf.Sin((Time.time + i * 0.2f) * 3f) + 5f) / 8f;

                        Color color = Color.yellow;
                        if (path.Count - i > Config.Temporary.selectedUnit.Moves + 1)
                        {
                            color = Color.red;
                        }
                        
                        Gizmos.DrawTile(path[i].transform.position + Vector3.up * 0.05f, WorldManager.InnerRadius * size, color);
                    }
                }
            }
            if(Input.GetMouseButtonUp(1))
            {
                if(tile)
                {
                    int movesToSub = 0;
                    for (int i = 0; i < path.Count; i++)
                    {
                        if (i <= Config.Temporary.selectedUnit.Moves)
                        {
                            movesToSub++;
                            Config.Temporary.selectedUnit.Tile = path[(path.Count - 1) - i];
                            Config.Temporary.selectedTile = path[(path.Count - 1)- i];
                        }
                    }

                    Config.Temporary.selectedUnit.Moves -= movesToSub - 1;
                    CanvasLayerInGame.RefreshSelectedTile();
                }
            }
        }

        lastTile = tile;
    }

    public static void Play()
    {
        Config.GameSave = new ConfigGameSave
        {
            nation = Config.GameSave.nation,
            map = Config.GameSave.map
        };

        MapManager.Load(Config.GameSave.map);
        WorldManager.Generate(MapManager.LoadedMap);
        CanvasManager.Layer = "InGame";

        CameraManager.Focus();
        CanvasLayerInGame.RefreshSelectedTile();
        CanvasPanelMap.Refresh();
    }

    public void NextTurn()
    {
        var turnWaiters = FindObjectsOfType<MonoBehaviour>();
        for (int i = 0; i < turnWaiters.Length; i++)
        {
            if(turnWaiters[i] is ITurnWaiter)
            {
                ((ITurnWaiter)turnWaiters[i]).NewTurn();
            }
        }

        CanvasLayerInGame.RefreshSelectedTile();
        Config.GameSave.turn++;

        Config.GameSave.science += Config.GameSave.sciencePerTurn;
        Config.GameSave.gold += Config.GameSave.goldPerTurn;
        Config.GameSave.faith += Config.GameSave.faithPerTurn;
        Config.GameSave.culture += Config.GameSave.culturePerTurn;
    }

    public static UnitObject PlaceUnit(string name, TileObject tile, Nation nation)
    {
        if (!instance) instance = FindObjectOfType<GameManager>();

        UnitObject unitObject = Instantiate(instance.unitPrefab).GetComponent<UnitObject>();
        unitObject.Initialize(name, tile, nation);

        return unitObject;
    }

    public static ImprovementObject PlaceImprovement(string name, TileObject tile, UnitObject source)
    {
        if(name == "City")
        {
            //capital doesnt exist yet, but its building a city
            if(!source.Nation.Capital)
            {
                name = "Capital";
            }
        }
        else if(name == "Capital")
        {
            //building a capital, but we already have one
            if(source.Nation.Capital)
            {
                name = "City";
            }
        }

        Debug.Log("Placed " + name);
        Improvement improvement = name;

        ImprovementObject improvementObject = Instantiate(improvement.prefab).GetComponent<ImprovementObject>();
        improvementObject.Improvement = improvement;
        improvementObject.Tile = tile;
        improvementObject.Initialize(source);

        return improvementObject;
    }
}
