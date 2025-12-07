using UnityEngine;

/// <summary>
/// Tile for Start in Dungeon
/// </summary>
[CreateAssetMenu(menuName = "_SO/Dungeon/Start")]
public class StartTileDef : BaseTileDef, IDungeonTileOnLand
{
    [SerializeField] private float healPercent;

    /// <summary>
    /// Event when landed on
    /// </summary>
    public void OnLand(DungeonHandler dungeon, int tileIndex)
    {
        Debug.Log("Landed on Start tile!");
    }
}