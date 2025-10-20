using UnityEngine;

/// <summary>
/// Tile for Resting in Dungeons
/// </summary>
[CreateAssetMenu(menuName = "_SO/Dungeon/Rest")]
public class RestTileDef : BaseTileDef
{
    [SerializeField] private RestChoices restChoices;

    public RestChoices Choices => restChoices;
}