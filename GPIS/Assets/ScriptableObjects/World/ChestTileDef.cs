using UnityEngine;

/// <summary>
/// Tile for Dungeon Chests
/// </summary>
[CreateAssetMenu(menuName = "_SO/Dungeon/Chest")]
public class ChestTileDef : BaseTileDef
{
    [SerializeField] private Quality quality;
    public Quality ChestQuality => quality;
}
