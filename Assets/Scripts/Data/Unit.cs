using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class Unit : ScriptableObject, IProducible
{
    public List<Task> tasks = new List<Task>();
    public int moves = 2;
    public int range = 4;
    public int maintenance = 1;

    public int costInGold = 100;
    public int costInProduction = 50;
    public int costInFaith = 100;

    int IProducible.CostInFaith()
    {
        return costInFaith;
    }

    int IProducible.CostInGold()
    {
        return costInGold;
    }

    int IProducible.CostInProduction()
    {
        return costInProduction;
    }

    public static implicit operator Unit(string name)
    {
        if (!GameManager.instance) GameManager.instance = FindObjectOfType<GameManager>();

        for (int i = 0; i < GameManager.instance.units.Count;i++)
        {
            if(GameManager.instance.units[i].name == name)
            {
                return GameManager.instance.units[i];
            }
        }

        return null;
    }
}
