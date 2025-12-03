using UnityEngine;

/// <summary>
/// Tile for Goal in Dungeon
/// </summary>
[CreateAssetMenu(menuName = "_SO/Dungeon/Goal")]
public class GoalTileDef : BaseTileDef, IDungeonTileOnLand
{
    [SerializeField] private GoalChoices choices;
    public GoalChoices Choices => choices;

    /// <summary>
    /// Event when landed on
    /// </summary>
    public void OnLand(DungeonHandler dungeon, int tileIndex)
    {
        Debug.Log("Landed on Goal tile!");
    }
}