using UnityEngine;
using UnityEngine.Tilemaps;

/// <summary>
/// Base Class for all dungeon tiles
/// </summary>
public abstract class BaseTileDef: ScriptableObject
{
    [SerializeField] private string id;
    [SerializeField] private string tileName;
    [SerializeField] private TileBase tile;
    public string ID => id;
    public string TileName => tileName;
    public TileBase Tile => tile;
}

public class DungeonTile
{
    public BaseTileDef def;
    public Vector3Int pos;
    public bool visited = false;

}