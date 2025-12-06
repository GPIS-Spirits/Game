using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Tile for Dungeon Battles
/// </summary>
[CreateAssetMenu(menuName = "_SO/Dungeon/Battle")]
public class BattleTileDef : BaseTileDef, IDungeonTileOnLand
{
    [SerializeField]
    private List<EnemyDef> enemies;
    public IReadOnlyList<EnemyDef> Enemies => enemies;
    /// <summary>
    /// Event when landed on
    /// </summary>
    public void OnLand(DungeonHandler dungeon, int tileIndex)
    {
        Debug.Log("Landed on Battle tile!");
    }
}
