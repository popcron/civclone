using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class Task : ScriptableObject
{    
    [Serializable]
    public struct Action
    {
        public ActionType type;
        public string value;
    }

    public enum ActionType
    {
        CreateImprovement,
        DestroyImprovement,
        DestroySelf,
        DamageVictim,
        DamageSelf,
        HealSelf,
        MoveSelf,
        ClaimTerritory,
        ProduceUnit,
        ProduceBuilding,
        RepairImprovement,
        PilageImprovement
    }

    public string friendlyName = "Task";
    public bool loseTurn;
    public List<Action> actions = new List<Action>();
}