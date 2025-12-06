using UnityEngine;

/// <summary>
/// Tile for Dungeon Chests
/// </summary>
[CreateAssetMenu(menuName = "_SO/Dungeon/Chest")]
public class ChestTileDef : BaseTileDef, IDungeonTileOnLand
{
    [SerializeField] private Quality quality;
    public Quality ChestQuality => quality;

    /// <summary>
    /// Event when landed on
    /// </summary>
    public void OnLand(DungeonHandler dungeon, int tileIndex)
    {
        Debug.Log("Landed on Chest tile!");
    }
}
