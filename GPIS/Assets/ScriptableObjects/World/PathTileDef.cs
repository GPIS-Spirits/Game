using UnityEngine;

/// <summary>
/// Tile for Path in Dungeons
/// </summary>
[CreateAssetMenu(menuName = "_SO/Dungeon/Path")]
public class PathTileDef : BaseTileDef
{
    [SerializeField] private int movementCost;
    public int MovementCost => movementCost;
}