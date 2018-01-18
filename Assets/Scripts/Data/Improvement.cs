using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct PerTurnStatistic
{
    public Resource resource;
    public int amount;

    public override string ToString()
    {
        return resource.name + " +" + amount;
    }
}

[CreateAssetMenu]
public class Improvement : ScriptableObject
{
    public List<PerTurnStatistic> stats = new List<PerTurnStatistic>();
    public int maintenance = 0;
    public List<Task> tasks = new List<Task>();
    public GameObject prefab;

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

    public static implicit operator Improvement(string name)
    {
        if (!GameManager.instance) GameManager.instance = FindObjectOfType<GameManager>();

        for (int i = 0; i < GameManager.instance.improvements.Count; i++)
        {
            if (GameManager.instance.improvements[i].name == name)
            {
                return GameManager.instance.improvements[i];
            }
        }

        return null;
    }
}
