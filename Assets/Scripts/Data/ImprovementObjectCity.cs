using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImprovementObjectCity : ImprovementObject, ITurnWaiter
{
    public string cityName = "City";
    public int cityLevel = 1;
    public int cityLevelProgress = 0;

    public List<Building> buildings = new List<Building>();
    public ScriptableObject producing;
    public int producingProgress;

    protected List<UnitObject> unitObjects = new List<UnitObject>();
    protected List<TileObject> territory = new List<TileObject>();

    public int LevelProgressRequired
    {
        get
        {
            return 10 + ((cityLevel - 1) * 8);
        }
    }

    public List<TileObject> InnerBounds
    {
        get
        {
            List<TileObject> tiles = new List<TileObject>();

            for (int i = 0; i < territory.Count; i++)
            {
                if (territory[i].Nation == this)
                {
                    var neighbours = territory[i].Neighbours;
                    for (int n = 0; n < neighbours.Count; n++)
                    {
                        if (neighbours[n].Nation != this)
                        {
                            tiles.Add(territory[i]);
                            break;
                        }
                    }
                }
            }

            return tiles;
        }
    }

    public List<TileObject> OuterBounds
    {
        get
        {
            List<TileObject> tiles = new List<TileObject>();

            for (int i = 0; i < territory.Count; i++)
            {
                if (territory[i].Nation == this)
                {
                    var neighbours = territory[i].Neighbours;
                    for (int n = 0; n < neighbours.Count; n++)
                    {
                        if (neighbours[n].Nation != this)
                        {
                            if (!tiles.Contains(neighbours[n]))
                            {
                                tiles.Add(neighbours[n]);
                            }
                        }
                    }
                }
            }

            return tiles;
        }
    }

    public override int GetStatistic(string statistic)
    {
        int amount = 0;

        for (int i = 0; i < territory.Count; i++)
        {
            amount += territory[i].GetStatistic(statistic);
        }

        for (int i = 0; i < buildings.Count; i++)
        {
            amount += buildings[i].GetStatistic(statistic);
        }

        if(statistic == "Gold")
        {
            for (int i = 0; i < unitObjects.Count; i++)
            {
                amount -= unitObjects[i].Unit.maintenance;
            }
        }

        return amount;
    }

    protected override void Awake()
    {
        if (!WorldManager.instance) WorldManager.instance = FindObjectOfType<WorldManager>();
        WorldManager.instance.cities.Add(this);
    }

    public override void Initialize(UnitObject source)
    {
        base.Initialize(source);

        cityName = source.Nation.cityNames[source.Nation.Cities.Count % source.Nation.cityNames.Count];
        Nation = source.Nation;

        Claim(Tile);
        Claim(Tile.Left);
        Claim(Tile.Right);
        Claim(Tile.TopLeft);
        Claim(Tile.TopRight);
        Claim(Tile.BottomLeft);
        Claim(Tile.BottomRight);
    }

    public static ImprovementObjectCity FindNearest(TileObject tile, Nation nation)
    {
        var cities = nation.Cities;
        int lastCost = int.MaxValue;
        int index = 0;
        for (int i = 0; i < cities.Count; i++)
        {
            var newPath = WorldPathfinding.FindPath(tile, cities[i].Tile);
            if(newPath.TotalCost < lastCost)
            {
                index = i;
                lastCost = newPath.TotalCost;
            }
        }

        return cities[index];
    }

    public void Remove(UnitObject unitObject)
    {
        unitObjects.Remove(unitObject);
    }

    public void Expand()
    {
        var neighbours = Nation.OuterBounds;
        TileObject randomTile = neighbours[UnityEngine.Random.Range(0, neighbours.Count)];
        Claim(randomTile);
    }

    public void Claim(TileObject tile)
    {
        if (tile)
        {
            if(tile.Nation && tile.City)
            {
                tile.City.Unclaim(tile);
            }

            tile.SetNation(Nation, this);
            territory.Add(tile);

            if(Nation.Capital)
            {
                Nation.Capital.UpdateTerritory();
            }
        }
    }

    public void Unclaim(TileObject tile)
    {
        if(tile)
        {
            tile.SetNation(null, null);
            territory.Remove(tile);

            if (Nation.Capital)
            {
                Nation.Capital.UpdateTerritory();
            }
        }
    }

    public override void PerformAction(Task task)
    {
        base.PerformAction(task);

        for (int i = 0; i < task.actions.Count; i++)
        {
            switch (task.actions[i].type)
            {
                case Task.ActionType.ProduceBuilding:
                    ProduceBuilding(task.actions[i].value);
                    break;
                case Task.ActionType.ProduceUnit:
                    ProduceUnit(task.actions[i].value);
                    break;
            }
        }
    }

    private void ProduceBuilding(string building)
    {
        producing = (Building)building;
        producingProgress = 0;
    }

    private void ProduceUnit(string unit)
    {
        producing = (Unit)unit;
        producingProgress = 0;
    }
    
    internal override string GetName()
    {
        return cityLevel + " " + cityName;
    }

    internal override string GetStats()
    {
        string stats = "";

        if (producing)
        {
            IProducible producible = (IProducible)producing;
            if(producible != null)
            {
                int turnsLeft = Mathf.CeilToInt((producible.CostInProduction() - producingProgress) / (float)GetStatistic("Production"));
                stats = "Producing " + producing.name + " (" + turnsLeft + " turn(s) left)";
            }
            else
            {
                stats = "Producing " + producing.name;
            }
        }

        if(stats != "")
        {
            stats += "\n";
        }

        stats += "Gold +" + GetStatistic("Gold");
        stats += "\n";
        stats += "Science +" + GetStatistic("Science");
        stats += "\n";
        stats += "Production +" + GetStatistic("Production");
        stats += "\n";
        stats += "Food +" + GetStatistic("Food");

        return stats;
    }

    void ITurnWaiter.NewTurn()
    {
        producingProgress += GetStatistic("Production");
        IProducible producible = (IProducible)producing;
        if(producible != null && producingProgress >= producible.CostInProduction())
        {
            if(producing is Unit)
            {
                var newUnit = GameManager.PlaceUnit(producing.name, Tile, Nation);
                unitObjects.Add(newUnit);
            }
            else if(producing is Building)
            {
                buildings.Add(producing.name);
            }

            producingProgress = 0;
            producing = null;
        }

        cityLevelProgress += GetStatistic("Food");
        if(cityLevelProgress >= LevelProgressRequired)
        {
            cityLevelProgress = 0;
            cityLevel++;
            Expand();
        }
    }
}
