using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitObject : MonoBehaviour, ITurnWaiter
{
    [SerializeField]
    private Nation nation;

    [SerializeField]
    private Unit unit;

    [SerializeField]
    private TileObject tile;

    [SerializeField]
    private int moves;

    [SerializeField]
    private ImprovementObjectCity city;
    
    public int Moves
    {
        get
        {
            return moves;
        }
        set
        {
            moves = value;
        }
    }

    public Nation Nation
    {
        get
        {
            return nation;
        }
    }

    public Unit Unit
    {
        get
        {
            return unit;
        }
    }

    public ImprovementObjectCity City
    {
        get
        {
            return city;
        }
    }

    public TileObject Tile
    {
        get
        {
            return tile;
        }
        set
        {
            if (tile) tile.Exit(this);

            tile = value;
            if (tile) tile.Enter(this);

            transform.position = tile.transform.position;
        }
    }

    private void OnDestroy()
    {
        if(Application.isPlaying)
        {
            if (City) City.Remove(this);
            if (tile) tile.Exit(this);
            if (Config.Temporary != null && Config.Temporary.selectedUnit == this)
            {
                Config.Temporary.selectedUnit = null;
                CanvasLayerInGame.RefreshSelectedTile();
            }
        }
    }

    public void ResetMoves()
    {
        moves = unit.moves;
    }

    public void Initialize(string unit, TileObject tile, Nation nation)
    {
        this.unit = unit;
        this.nation = nation;
        Tile = tile;
        city = tile.City;

        ResetMoves();
    }

    public void PerformAction(Task task)
    {
        Debug.Log("Perform Action");
        if(task.loseTurn)
        {
            moves = 0;
        }

        for (int i = 0; i < task.actions.Count; i++)
        {
            switch(task.actions[i].type)
            {
                case Task.ActionType.CreateImprovement:
                    GameManager.PlaceImprovement(task.actions[i].value, Tile, this);
                    break;
                case Task.ActionType.DestroySelf:
                    Destroy(gameObject);
                    break;
                case Task.ActionType.ClaimTerritory:
                    ClaimTile();
                    break;
            }
        }

        CanvasLayerInGame.RefreshSelectedTile();
    }

    private void ClaimTile()
    {
        var city = ImprovementObjectCity.FindNearest(Tile, Nation);
        if (city)
        {
            city.Claim(Tile);
        }
    }

    void ITurnWaiter.NewTurn()
    {
        ResetMoves();
    }
}
