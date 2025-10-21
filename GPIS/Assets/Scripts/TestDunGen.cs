using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class DungeonGenerator : MonoBehaviour
{
    [Header("Scene References")]
    [SerializeField] private Grid grid;
    [SerializeField] private Tilemap tilemap;

    [Header("Tiles")]
    [SerializeField] private PathTileDef startTile;
    [SerializeField] private GoalTileDef goalTile;
    [SerializeField] private List<BaseTileDef> middleTiles;

    [Header("Layout")]
    [SerializeField, Min(2)] private int length = 10;
    [SerializeField] private Vector3Int origin = new Vector3Int(0, 0, 0);
    [SerializeField] private Vector2Int step = new Vector2Int(1, 0);

    [Header("Randomization")]
    [SerializeField] private bool useFixedSeed = true;
    [SerializeField] private int seed = 12345;

    [Header("Runtime")]
    [SerializeField] private bool autoGenerateOnStart = true;

    private System.Random rng;

    private void Start()
    {
        if (autoGenerateOnStart)
            Generate();
    }

    [ContextMenu("Generate")]
    public void Generate()
    {
        if (grid == null) grid = GetComponentInParent<Grid>();
        if (tilemap == null) tilemap = GetComponentInChildren<Tilemap>();

        if (grid == null || tilemap == null)
        {
            Debug.LogError("DungeonGenerator needs Grid and Tilemap.");
            return;
        }
        if (startTile == null || startTile.Tile == null)
        {
            Debug.LogError("Start tile missing or has no Tile.");
            return;
        }
        if (goalTile == null || goalTile.Tile == null)
        {
            Debug.LogError("Goal tile missing or has no Tile.");
            return;
        }
        if (length < 2)
        {
            Debug.LogError("Length must be >= 2.");
            return;
        }

        rng = useFixedSeed ? new System.Random(seed) : new System.Random(Environment.TickCount);

        tilemap.ClearAllTiles();

        Vector3Int current = origin;
        tilemap.SetTile(current, startTile.Tile);

        for (int i = 1; i < length - 1; i++)
        {
            current += new Vector3Int(step.x, step.y, 0);
            var midDef = PickMiddle();
            var tile = (midDef != null) ? midDef.Tile : startTile.Tile;
            tilemap.SetTile(current, tile);
        }
        current += new Vector3Int(step.x, step.y, 0);
        tilemap.SetTile(current, goalTile.Tile);
    }

    private BaseTileDef PickMiddle()
    {
        if (middleTiles == null || middleTiles.Count == 0) return null;

        int idx = rng.Next(0, middleTiles.Count);
        var def = middleTiles[idx];
        if (def == null || def.Tile == null) return null;
        return def;
    }

    [ContextMenu("Clear")]
    public void Clear()
    {
        if (tilemap != null) tilemap.ClearAllTiles();
    }
}
