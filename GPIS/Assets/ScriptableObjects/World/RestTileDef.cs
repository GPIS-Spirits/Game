using UnityEngine;

/// <summary>
/// Tile for Resting in Dungeons
/// </summary>
[CreateAssetMenu(menuName = "_SO/Dungeon/Rest")]
public class RestTileDef : BaseTileDef, IDungeonTileOnLand
{
    [SerializeField] private RestChoices restChoices;

    public RestChoices Choices => restChoices;

    /// <summary>
    /// Event when landed on
    /// </summary>
    public void OnLand(DungeonHandler dungeon, int tileIndex)
    {
        Debug.Log("Landed on Rest tile!");
    }
}