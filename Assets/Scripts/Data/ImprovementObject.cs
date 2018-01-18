using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ImprovementObject : MonoBehaviour
{
    [SerializeField]
    private Improvement improvement;

    [SerializeField]
    private WorldTile tile;

    [SerializeField]
    private Nation nation;

    public WorldTile Tile
    {
        get
        {
            return tile;
        }
        set
        {
            if(tile)
            {
                for(int i = 0; i < tile.Improvements.Count;i++)
                {
                    if(tile.Improvements[i].gameObject == gameObject)
                    {
                        tile.Exit(this);
                    }
                }
            }

            tile = value;

            if (tile)
            {
                transform.position = tile.transform.position;
                for (int i = 0; i < tile.Improvements.Count; i++)
                {
                    if (tile.Improvements[i].gameObject == gameObject)
                    {
                        return;
                    }
                }
                tile.Enter(this);
            }
        }
    }

    public Improvement Improvement
    {
        get
        {
            return improvement;
        }
        set
        {
            improvement = value;
        }
    }

    public Nation Nation
    {
        get
        {
            return nation;
        }
        set
        {
            nation = value;
        }
    }

    private void OnDestroy()
    {
        if (Application.isPlaying)
        {
            if (tile) tile.Exit(this);
        }
    }

    protected virtual void Awake()
    {
        if (!WorldManager.instance) WorldManager.instance = FindObjectOfType<WorldManager>();
        WorldManager.instance.improvements.Add(this);
    }

    public virtual void Initialize(UnitObject source)
    {

    }

    public virtual void PerformAction(Task task)
    {
        for (int i = 0; i < task.actions.Count; i++)
        {
            switch (task.actions[i].type)
            {
                case Task.ActionType.DestroySelf:
                    Destroy(gameObject);
                    break;
            }
        }
    }

    public virtual int GetStatistic(string statistic)
    {
        int amount = 0;

        for (int i = 0; i < improvement.stats.Count; i++)
        {
            if(improvement.stats[i].resource.name == statistic)
            {
                amount += improvement.stats[i].amount;
            }
        }

        return amount;
    }

    internal virtual string GetName()
    {
        return improvement.name;
    }

    internal virtual string GetStats()
    {
        string[] array = new string[improvement.stats.Count];
        for (int i = 0; i < improvement.stats.Count; i++)
        {
            array[i] = improvement.stats[i].ToString();
        }

        return string.Join("\n", array);
    }
}
