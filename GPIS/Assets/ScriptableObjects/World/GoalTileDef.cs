using UnityEngine;

/// <summary>
/// Tile for Goal in Dungeon
/// </summary>
[CreateAssetMenu(menuName = "_SO/Dungeon/Goal")]
public class GoalTileDef : BaseTileDef
{
    [SerializeField] private GoalChoices choices;
    public GoalChoices Choices => choices;
}