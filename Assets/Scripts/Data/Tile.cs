using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Elevation
{
    Flat,
    Hill,
    Mountain
}

public enum Continent
{
    Nothnig,
    America,
    Asia,
    Africa,
    Europe
}

public class Tile
{
    public int x = 0;
    public int y = 0;

    public string terrainType;
    public string resourceType;
    public string firstFeature;
    public string secondFeature;
    public byte riverIndicator = 0;
    public Elevation elevation = Elevation.Flat;
    public Continent continent = Continent.Nothnig;

    public byte unknown;
}
