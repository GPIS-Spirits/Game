using UnityEngine;

/// <summary>
/// Tile for Path in Dungeons
/// </summary>
[CreateAssetMenu(menuName = "_SO/Dungeon/Path")]
public class PathTileDef : BaseTileDef, IDungeonTileOnLand
{
    [SerializeField] private int movementCost;
    public int MovementCost => movementCost;

    /// <summary>
    /// Event when landed on
    /// </summary>
    public void OnLand(DungeonHandler dungeon, int tileIndex)
    {
        Debug.Log("Landed on Path tile!");
    }
}