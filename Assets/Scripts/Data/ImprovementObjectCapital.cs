using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImprovementObjectCapital : ImprovementObjectCity
{
    private List<TileObject> allTerritory = new List<TileObject>();

    protected override void Awake()
    {
        if (!WorldManager.instance) WorldManager.instance = FindObjectOfType<WorldManager>();
        WorldManager.instance.capitals.Add(this);
        WorldManager.instance.cities.Add(this);
    }

    public void UpdateTerritory()
    {
        allTerritory.Clear();
        for (int i = 0; i < WorldManager.Tiles.Length; i++)
        {
            if (WorldManager.Tiles[i].Nation == Nation)
            {
                allTerritory.Add(WorldManager.Tiles[i]);
            }
        }
    }

    private void Update()
    {
        Gizmos.DrawTerritory(allTerritory, Nation.primaryColor, Nation.secondaryColor);
    }
}
