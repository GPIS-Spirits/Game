using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Interface || Calls event on landing.
/// Implemented by individual tiles
/// </summary>
public interface IDungeonTileOnLand
{
    void OnLand(DungeonHandler dungeon, int tileIndex);
}

public class DungeonHandler : MonoBehaviour
{
    [Header("Generation")]
    [Min(1)] public int tileCount = 10;
    public float tileSpacing = 1.5f;
    public GameObject tilePrefab;
    [SerializeField] private bool doStartTileLogic = false;

    [Header("Tiles")]
    [SerializeField] private BaseTileDef startTileDef;
    [SerializeField] private BaseTileDef goalTileDef;
    [SerializeField] private BaseTileDef[] middleTileDef;

    [Header("Player Marker")]
    public Transform playerMarker;
    [Min(0)] public int startIndex = 0;

    [Header("Jump Settings")]
    public float jumpDuration = 0.4f;
    public float jumpHeight = 1.0f;
    public float idleYOffset = 0.5f;

    private readonly List<TileInstance> _tiles = new();
    private int _currentIndex;
    private bool _isJumping;
    private Vector3 _baseScale = Vector3.one;


    /// <summary>
    /// Holds all the per-tile data
    /// Positiion, Tile Definition, Visited bool
    /// </summary>
    private class TileInstance
    {
        public BaseTileDef def;
        public Transform transform;
        public bool visited;
    }

    private void Awake()
    {
        if (playerMarker != null)
            _baseScale = playerMarker.localScale;

        GenerateTiles();
        SnapPlayerTo(startIndex);   // start without animation

        // Runs Start Tile Logic if =>
        if (doStartTileLogic)
        {
            SnapPlayerTo(startIndex);
            var startTile = _tiles[_currentIndex];
            startTile.visited = true;
            if (startTile.def is IDungeonTileOnLand startOnLand)
                startOnLand.OnLand(this, _currentIndex);
        }
    }

    /// <summary>
    /// Temporary Movement for our Dungeon, A-D or L-R
    /// </summary>
    private void Update()
    {
        // Locks movement during animations
        if (_isJumping) return;

        // Checks D and R arrow key to move right
        if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
            TryMove(+1);
        // Checks A and L arrow key to move Left
        else if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
            TryMove(-1);
    }

    /// <summary>
    /// Creates the Dungeon tile map
    /// Per tile events handled by SO script
    /// </summary>
    public void GenerateTiles()
    {
        ClearExistingTiles();

        if (tilePrefab == null || middleTileDef == null || middleTileDef.Length == 0)
            return;

        _tiles.Clear();

        for (int i = 0; i < tileCount; i++)
        {
            BaseTileDef def;

            bool isFirst = (i == 0);
            bool isLast = (i == tileCount - 1);

            if (isFirst && startTileDef != null)
            {
                // First tile (isFirst)
                def = startTileDef;
            }
            else if (isLast && goalTileDef != null)
            {
                // Last tile (isLast)
                def = goalTileDef;
            }
            else
            {
                // Picks a random tile. We can do weights later.
                def = middleTileDef[Random.Range(0, middleTileDef.Length)];
            }

            // Creates GO at adjusted position
            Vector3 worldPos = transform.position + new Vector3(i * tileSpacing, 0f, 0f);
            GameObject tileGO = Instantiate(tilePrefab, worldPos, Quaternion.identity, transform);

            // Sets sprite (background) and overlay (foreground, event) images
            var view = tileGO.GetComponent<TileView>();
            if (view != null)
            {
                if (view.baseRenderer != null)
                    view.baseRenderer.sprite = def.sprite;

                if (view.overlayRenderer != null)
                    view.overlayRenderer.sprite = def.overlay;
            }

            tileGO.name = string.IsNullOrEmpty(def.displayName)
                ? $"Tile_{i:00}"
                : $"{i:00}_{def.displayName}";

            // Adds the tile instance
            _tiles.Add(new TileInstance
            {
                def = def,
                transform = tileGO.transform,
                visited = false
            });
        }

        _currentIndex = Mathf.Clamp(_currentIndex, 0, _tiles.Count - 1);
    }


    /// <summary>
    /// Attempts to move || will not move if already moving
    /// </summary>
    private void TryMove(int delta)
    {
        if (_tiles.Count == 0 || playerMarker == null) return;

        int newIndex = Mathf.Clamp(_currentIndex + delta, 0, _tiles.Count - 1);
        if (newIndex == _currentIndex) return;

        StartCoroutine(JumpMove(newIndex, delta));
    }

    /// <summary>
    /// Controls player jump movement to next tile, or specified tile
    /// </summary>
    /// <returns></returns>
    private IEnumerator JumpMove(int newIndex, int direction)
    {
        _isJumping = true;

        Vector3 start = playerMarker.position;
        Vector3 targetBase = _tiles[newIndex].transform.position;
        Vector3 end = targetBase + Vector3.up * idleYOffset;

        // Flips to face direction of movement
        float sign = Mathf.Sign(direction);
        playerMarker.localScale = new Vector3(
            Mathf.Abs(_baseScale.x) * sign,
            _baseScale.y,
            _baseScale.z
        );

        float t = 0f;

        Quaternion startRot = playerMarker.rotation;
        
        // Determines front or backflips
        float spinDir = -1f * sign;

        while (t < 1f)
        {
            t += Time.deltaTime / jumpDuration;
            float clampedT = Mathf.Clamp01(t);

            // Jump Arc
            Vector3 pos = Vector3.Lerp(start, end, clampedT);
            float arc = 4f * clampedT * (1f - clampedT);
            pos.y += arc * jumpHeight;
            playerMarker.position = pos;

            // Basically a Lerp, for the spin.
            float angle = 360f * clampedT * (spinDir);
            playerMarker.rotation = startRot * Quaternion.Euler(0f, 0f, angle);

            yield return null;
        }

        // Ends by snapping; just in case.
        _currentIndex = newIndex;
        playerMarker.position = end;
        playerMarker.rotation = startRot;
        _isJumping = false;

        // onLand() called and tile event triggers, sets to visited [single visit]
        var tile = _tiles[_currentIndex];

        if (!tile.visited)
        {
            tile.visited = true;

            if (tile.def is IDungeonTileOnLand onLand)
                onLand.OnLand(this, _currentIndex);
        }
    }

    // Snap, used in start; can be used to Teleport later
    private void SnapPlayerTo(int index)
    {
        if (_tiles.Count == 0 || playerMarker == null) return;

        // Ensures no out of ranges
        index = Mathf.Clamp(index, 0, _tiles.Count - 1);
        _currentIndex = index;

        // Sets position
        Vector3 basePos = _tiles[index].transform.position;
        playerMarker.position = basePos + Vector3.up * idleYOffset;
    }

    private void ClearExistingTiles()
    {
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            var child = transform.GetChild(i);
            // Unity idiom || Destroys in editor if it's in editor mode via immediate
        #if UNITY_EDITOR
            if (!Application.isPlaying)
                DestroyImmediate(child.gameObject);
            else
                Destroy(child.gameObject);
        #else
            Destroy(child.gameObject);
        #endif
        }
    }
}
