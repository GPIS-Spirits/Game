using UnityEngine;
using UnityEngine.Tilemaps;

/// <summary>
/// Base Class for all dungeon tiles
/// </summary>
public abstract class BaseTileDef : ScriptableObject
{
    [Header("Identity")]
    public string id;
    public string displayName;
    [TextArea] public string description;

    [Header("Art")]
    public Sprite sprite;
}