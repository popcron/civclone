using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class Nation : ScriptableObject
{
    public List<string> cityNames = new List<string>();
    public string citizenNoun = "Nationese";

    public Color primaryColor = Color.white;
    public Color secondaryColor = Color.gray;

    public int GetStatistic(string statistic)
    {
        int amount = 0;
        var cities = Cities;

        for(int i = 0; i < cities.Count;i++)
        {
            amount += cities[i].GetStatistic(statistic);
        }

        return amount;
    }

    public ImprovementObjectCapital Capital
    {
        get
        {
            if (!WorldManager.instance) WorldManager.instance = FindObjectOfType<WorldManager>();
            
            for (int i = 0; i < WorldManager.instance.capitals.Count; i++)
            {
                if (WorldManager.instance.capitals[i].Nation == this)
                {
                    return WorldManager.instance.capitals[i];
                }
            }

            return null;
        }
    }

    public List<ImprovementObjectCity> Cities
    {
        get
        {
            if (!WorldManager.instance) WorldManager.instance = FindObjectOfType<WorldManager>();
            List<ImprovementObjectCity> cities = new List<ImprovementObjectCity>();

            for (int i = 0; i < WorldManager.instance.cities.Count; i++)
            {
                if (WorldManager.instance.cities[i].Nation == this)
                {
                    cities.Add(WorldManager.instance.cities[i]);
                }
            }

            return cities;
        }
    }

    public List<TileObject> InnerBounds
    {
        get
        {
            List<TileObject> tiles = new List<TileObject>();

            for(int i = 0; i < WorldManager.Tiles.Length;i++)
            {
                if (WorldManager.Tiles[i].Nation == this)
                {
                    var neighbours = WorldManager.Tiles[i].Neighbours;
                    for (int n = 0; n < neighbours.Count; n++)
                    {
                        if(neighbours[n].Nation != this)
                        {
                            tiles.Add(WorldManager.Tiles[i]);
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

            for (int i = 0; i < WorldManager.Tiles.Length; i++)
            {
                if (WorldManager.Tiles[i].Nation == this)
                {
                    var neighbours = WorldManager.Tiles[i].Neighbours;
                    for (int n = 0; n < neighbours.Count; n++)
                    {
                        if (neighbours[n].Nation != this)
                        {
                            if(!tiles.Contains(neighbours[n]))
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

    public static implicit operator Nation(string name)
    {
        if (!GameManager.instance) GameManager.instance = FindObjectOfType<GameManager>();

        for (int i = 0; i < GameManager.instance.nations.Count; i++)
        {
            if (GameManager.instance.nations[i].name == name)
            {
                return GameManager.instance.nations[i];
            }
        }

        return null;
    }
}
