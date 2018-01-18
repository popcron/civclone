using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class Biome : ScriptableObject
{
    [Range(0f, 1f)]
    public float minMoisture = 0f;
    [Range(0f, 1f)]
    public float maxMoisture = 1f;
    [Range(0f, 1f)]
    public float minElevation = 0f;
    [Range(0f, 1f)]
    public float maxElevation = 1f;

    public Color color = Color.white;
}
