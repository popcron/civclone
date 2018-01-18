using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldTile : MonoBehaviour
{
    public HexCoordinate coordinate;
    public Color color = Color.white;
    public bool passable = true;

    private List<UnitObject> unitObjects = new List<UnitObject>();
    private List<ImprovementObject> improvementObjects = new List<ImprovementObject>();
    private WorldTile topLeft;
    private WorldTile topRight;
    private WorldTile left;
    private WorldTile right;
    private WorldTile bottomLeft;
    private WorldTile bottomRight;
    private Nation nation;
    private ImprovementObjectCity city;

    public List<UnitObject> Units
    {
        get
        {
            return unitObjects;
        }
    }

    public List<ImprovementObject> Improvements
    {
        get
        {
            return improvementObjects;
        }
    }

    public ImprovementObjectCity City
    {
        get
        {
            return city;
        }
    }

    public Nation Nation
    {
        get
        {
            return nation;
        }
    }

    public WorldTile TopLeft
    {
        get
        {
            return topLeft;
        }
    }

    public void SetNation(Nation nation, ImprovementObjectCity city)
    {
        this.nation = nation;
        this.city = city;
    }

    public WorldTile TopRight
    {
        get
        {
            return topRight;
        }
    }

    public WorldTile Left
    {
        get
        {
            return left;
        }
    }

    public WorldTile Right
    {
        get
        {
            return right;
        }
    }

    public WorldTile BottomLeft
    {
        get
        {
            return bottomLeft;
        }
    }

    public WorldTile BottomRight
    {
        get
        {
            return bottomRight;
        }
    }

    public WorldTile RandomNeighbour
    {
        get
        {
            var neighbours = Neighbours;
            return neighbours[UnityEngine.Random.Range(0, neighbours.Count)];
        }
    }

    public List<WorldTile> Neighbours
    {
        get
        {
            List<WorldTile> neighbours = new List<WorldTile>();
            if (topLeft) neighbours.Add(topLeft);
            if (topRight) neighbours.Add(topRight);
            if (left) neighbours.Add(left);
            if (right) neighbours.Add(right);
            if (bottomLeft) neighbours.Add(bottomLeft);
            if (bottomRight) neighbours.Add(bottomRight);

            return neighbours;
        }
    }

    public int GetStatistic(string statistic)
    {
        int amount = 0;
        for (int m = 0; m < Improvements.Count; m++)
        {
            amount += Improvements[m].Improvement.GetStatistic(statistic);
        }

        return amount;
    }

    internal void Exit(UnitObject obj)
    {
        if (!unitObjects.Contains(obj)) return;

        unitObjects.Remove(obj);
    }

    internal void Enter(UnitObject obj)
    {
        if (unitObjects.Contains(obj)) return;

        unitObjects.Add(obj);
    }

    internal void Exit(ImprovementObject obj)
    {
        if (!improvementObjects.Contains(obj)) return;

        improvementObjects.Remove(obj);
    }

    internal void Enter(ImprovementObject obj)
    {
        if (improvementObjects.Contains(obj)) return;

        improvementObjects.Add(obj);
    }

    public void RefreshNeighbors()
    {
        topLeft = WorldManager.Get(coordinate.X - 1, coordinate.Z + 1);
        topRight = WorldManager.Get(coordinate.X, coordinate.Z + 1);

        left = WorldManager.Get(coordinate.X - 1, coordinate.Z);
        right = WorldManager.Get(coordinate.X + 1, coordinate.Z);

        bottomLeft = WorldManager.Get(coordinate.X, coordinate.Z - 1);
        bottomRight = WorldManager.Get(coordinate.X + 1, coordinate.Z - 1);
    }
}
