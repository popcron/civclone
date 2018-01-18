using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IProducible
{
    int CostInGold();
    int CostInProduction();
    int CostInFaith();
}

[CreateAssetMenu]
public class Building : ScriptableObject, IProducible
{
    public List<PerTurnStatistic> stats = new List<PerTurnStatistic>();
    public int maintenance = 1;

    public int costInGold = 100;
    public int costInProduction = 50;
    public int costInFaith = 100;

    public int GetStatistic(string statistic)
    {
        int amount = 0;
        for (int s = 0; s < stats.Count; s++)
        {
            if (stats[s].resource.name == statistic)
            {
                amount += stats[s].amount;
            }
        }

        if (statistic == "Gold")
        {
            amount -= maintenance;
        }

        return amount;
    }

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

    public static implicit operator Building(string name)
    {
        if (!GameManager.instance) GameManager.instance = FindObjectOfType<GameManager>();

        for (int i = 0; i < GameManager.instance.buildings.Count; i++)
        {
            if (GameManager.instance.buildings[i].name == name)
            {
                return GameManager.instance.buildings[i];
            }
        }

        return null;
    }
}
