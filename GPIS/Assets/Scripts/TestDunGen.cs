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
    [SerializeField] private GoalTileDef startTile;              // single start/goal location
    [SerializeField] private List<BaseTileDef> middleTiles;      // optional decorations placed on top
    [SerializeField] private List<PathTileDef> pathOptions;      // random path choices for the loop

    [Header("Loop Shape (Perlin)")]
    [SerializeField, Min(8)] private int loopSegments = 64;      // number of samples around the loop
    [SerializeField, Min(1)] private int baseRadius = 8;         // base ring radius (in tiles)
    [SerializeField, Min(0)] private int noiseAmplitude = 4;     // how far radius can vary from base
    [SerializeField, Min(0.01f)] private float noiseScale = 0.6f;// Perlin frequency (bigger = faster wobble)
    [SerializeField, Min(1)] private int thickness = 1;          // ring thickness (draw a disk around each point)

    [Header("Placement / Origin")]
    [SerializeField] private Vector3Int origin = new Vector3Int(0, 0, 0); // loop center in tile coords

    [Header("Decoration")]
    [Range(0f, 1f)]
    [SerializeField] private float middleTileChance = 0.12f;     // chance to place a middle tile on a loop cell

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
        if (!HasAtLeastOneValidPathTile())
        {
            Debug.LogError("pathOptions must contain at least one PathTileDef with a valid Tile.");
            return;
        }
        if (loopSegments < 8)
        {
            Debug.LogError("loopSegments must be >= 8.");
            return;
        }

        rng = useFixedSeed ? new System.Random(seed) : new System.Random(Environment.TickCount);
        tilemap.ClearAllTiles();

        // Random Perlin offsets
        float ox = (float)rng.NextDouble() * 1000f;
        float oy = (float)rng.NextDouble() * 1000f;

        // Build loop
        List<Vector3Int> ringPoints = SamplePerlinLoop(ox, oy);

        HashSet<Vector3Int> loopCells = new HashSet<Vector3Int>();
        for (int i = 0; i < ringPoints.Count; i++)
        {
            Vector3Int a = ringPoints[i];
            Vector3Int b = ringPoints[(i + 1) % ringPoints.Count];
            foreach (var p in Bresenham(a, b))
            {
                foreach (var q in Disk(p, thickness))
                    loopCells.Add(q);
            }
        }

        // Choose a single start/goal cell (first point for determinism)
        Vector3Int startPos = ringPoints.Count > 0 ? ringPoints[0] : origin;

        // Paint path tiles randomly on all loop cells except the startPos
        foreach (var cell in loopCells)
        {
            if (cell == startPos) continue;
            var pathTile = PickPathTile();
            if (pathTile != null)
                tilemap.SetTile(cell, pathTile);
        }

        // Place the single start/goal tile last so it stays visible
        tilemap.SetTile(startPos, startTile.Tile);

        if (middleTiles != null && middleTiles.Count > 0 && middleTileChance > 0f)
        {
            foreach (var cell in loopCells)
            {
                if (cell == startPos) continue;
                if (rng.NextDouble() <= middleTileChance)
                {
                    var def = PickMiddle();
                    if (def != null && def.Tile != null)
                        tilemap.SetTile(cell, def.Tile);
                }
            }
        }

        tilemap.SetTile(startPos, startTile.Tile);
    }

    private bool HasAtLeastOneValidPathTile()
    {
        if (pathOptions == null || pathOptions.Count == 0) return false;
        foreach (var p in pathOptions)
        {
            if (p != null && p.Tile != null) return true;
        }
        return false;
    }

    private TileBase PickPathTile()
    {
        // Gather valid tiles once per call set; for simplicity, pick by rejection sampling
        if (pathOptions == null || pathOptions.Count == 0) return null;

        // Try a few times to avoid null entries
        for (int tries = 0; tries < 8; tries++)
        {
            int idx = rng.Next(0, pathOptions.Count);
            var def = pathOptions[idx];
            if (def != null && def.Tile != null)
                return def.Tile;
        }

        // Fallback: linear scan
        foreach (var def in pathOptions)
            if (def != null && def.Tile != null)
                return def.Tile;

        return null;
    }

    private List<Vector3Int> SamplePerlinLoop(float ox, float oy)
    {
        var pts = new List<Vector3Int>();
        var seen = new HashSet<Vector3Int>();

        for (int i = 0; i < loopSegments; i++)
        {
            float t = (float)i / loopSegments;
            float theta = t * Mathf.PI * 2f;

            float nx = Mathf.Cos(theta) * noiseScale + ox;
            float ny = Mathf.Sin(theta) * noiseScale + oy;
            float noise01 = Mathf.PerlinNoise(nx, ny);

            float radialOffset = (noise01 * 2f - 1f) * noiseAmplitude;
            float r = Mathf.Max(1f, baseRadius + radialOffset);

            float fx = origin.x + r * Mathf.Cos(theta);
            float fy = origin.y + r * Mathf.Sin(theta);
            var cell = new Vector3Int(Mathf.RoundToInt(fx), Mathf.RoundToInt(fy), 0);

            if (seen.Add(cell))
                pts.Add(cell);
        }

        if (pts.Count < 8)
        {
            pts.Clear();
            seen.Clear();
            for (int i = 0; i < loopSegments; i++)
            {
                float theta = (float)i / loopSegments * Mathf.PI * 2f;
                float fx = origin.x + baseRadius * Mathf.Cos(theta);
                float fy = origin.y + baseRadius * Mathf.Sin(theta);
                var cell = new Vector3Int(Mathf.RoundToInt(fx), Mathf.RoundToInt(fy), 0);
                if (seen.Add(cell))
                    pts.Add(cell);
            }
        }

        return pts;
    }

    private BaseTileDef PickMiddle()
    {
        if (middleTiles == null || middleTiles.Count == 0) return null;
        int idx = rng.Next(0, middleTiles.Count);
        var def = middleTiles[idx];
        return (def != null && def.Tile != null) ? def : null;
    }

    private IEnumerable<Vector3Int> Bresenham(Vector3Int a, Vector3Int b)
    {
        int x0 = a.x, y0 = a.y;
        int x1 = b.x, y1 = b.y;

        int dx = Mathf.Abs(x1 - x0);
        int sx = x0 < x1 ? 1 : -1;
        int dy = -Mathf.Abs(y1 - y0);
        int sy = y0 < y1 ? 1 : -1;
        int err = dx + dy;

        while (true)
        {
            yield return new Vector3Int(x0, y0, 0);
            if (x0 == x1 && y0 == y1) break;
            int e2 = 2 * err;
            if (e2 >= dy) { err += dy; x0 += sx; }
            if (e2 <= dx) { err += dx; y0 += sy; }
        }
    }

    private IEnumerable<Vector3Int> Disk(Vector3Int center, int r)
    {
        if (r <= 1)
        {
            yield return center;
            yield break;
        }

        int rr = r * r;
        for (int dy = -r; dy <= r; dy++)
        {
            for (int dx = -r; dx <= r; dx++)
            {
                if (dx * dx + dy * dy <= rr)
                    yield return new Vector3Int(center.x + dx, center.y + dy, 0);
            }
        }
    }

    [ContextMenu("Clear")]
    public void Clear()
    {
        if (tilemap != null)
            tilemap.ClearAllTiles();
    }
}
