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

    [SerializeField] private Transform[] flipTargets;
    [SerializeField] private float turnSpinDuration = 0.15f;
    private float _facing = +1f;
    private bool _isTurning = false;
    private Coroutine _turnRoutine;
    private Vector3[] _flipBaseScales;

    [Min(0)] public int startIndex = 0;

    [Header("Jump Settings")]
    public float jumpDuration = 0.4f;
    public float jumpHeight = 1.0f;
    public float idleYOffset = 0.5f;

    private readonly List<TileInstance> _tiles = new();
    private int _currentIndex;
    private bool _isJumping;
    private Vector3 _baseScale = Vector3.one;


    // This is used to prevent a/d movement
    public ElementalView elementalView;
    
    /// <summary>
    /// Holds all the per-tile data
    /// Position, Tile Definition, Visited bool
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

        if (flipTargets != null && flipTargets.Length > 0)
        {
            _flipBaseScales = new Vector3[flipTargets.Length];
            for (int i = 0; i < flipTargets.Length; i++)
                _flipBaseScales[i] = flipTargets[i] != null ? flipTargets[i].localScale : Vector3.one;
        }

        GenerateTiles();
        SnapPlayerTo(startIndex);

        if (doStartTileLogic)
        {
            SnapPlayerTo(startIndex);
            var startTile = _tiles[_currentIndex];
            startTile.visited = true;
            if (startTile.def is IDungeonTileOnLand startOnLand)
                startOnLand.OnLand(this, _currentIndex);
        }
    }

    private void Update()
    {
        if (elementalView.isOpen == true) return;

        if (_isJumping || _isTurning) return;

        if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
            TryMove(+1);
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
                def = startTileDef;
            else if (isLast && goalTileDef != null)
                def = goalTileDef;
            else
                def = middleTileDef[Random.Range(0, middleTileDef.Length)];

            Vector3 worldPos = transform.position + new Vector3(i * tileSpacing, 0f, 0f);
            GameObject tileGO = Instantiate(tilePrefab, worldPos, Quaternion.identity, transform);

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

        float desiredFacing = Mathf.Sign(delta);

        // Turning takes priority before movement
        if (desiredFacing != _facing)
        {
            if (_turnRoutine != null)
                StopCoroutine(_turnRoutine);

            _turnRoutine = StartCoroutine(TurnPlayer(desiredFacing, newIndex));
            return;
        }

        StartCoroutine(JumpMove(newIndex, delta));
    }

    /// <summary>
    /// Spins flip targets 180 degrees on Y-axis before movement
    /// </summary>
    private IEnumerator TurnPlayer(float desiredFacing, int nextIndex)
    {
        _isTurning = true;

        Quaternion[] startRots = new Quaternion[flipTargets.Length];
        Quaternion[] endRots = new Quaternion[flipTargets.Length];

        for (int i = 0; i < flipTargets.Length; i++)
        {
            Transform tx = flipTargets[i];
            if (tx == null) continue;

            startRots[i] = tx.rotation;

            float yRot = desiredFacing > 0 ? 0f : 180f;
            endRots[i] = Quaternion.Euler(0f, yRot, 0f);
        }

        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / turnSpinDuration;

            for (int i = 0; i < flipTargets.Length; i++)
            {
                Transform tr = flipTargets[i];
                if (tr == null) continue;
                tr.rotation = Quaternion.Slerp(startRots[i], endRots[i], t);
            }

            yield return null;
        }

        _facing = desiredFacing;
        _isTurning = false;

        StartCoroutine(JumpMove(nextIndex, (int)desiredFacing));
    }

    /// <summary>
    /// Controls player jump movement to next tile, with spin animations
    /// </summary>
    private IEnumerator JumpMove(int newIndex, int direction)
    {
        _isJumping = true;

        Vector3 start = playerMarker.position;
        Vector3 targetBase = _tiles[newIndex].transform.position;
        Vector3 end = targetBase + Vector3.up * idleYOffset;

        // Force player Z
        start.z = -5f;
        end.z = -5f;

        Transform[] spinSet = flipTargets;
        Quaternion[] startRots = new Quaternion[spinSet.Length];

        for (int i = 0; i < spinSet.Length; i++)
            startRots[i] = spinSet[i] != null ? spinSet[i].rotation : Quaternion.identity;

        float sign = Mathf.Sign(direction);
        float tLerp = 0f;
        float spinDir = -1f * sign;

        while (tLerp < 1f)
        {
            tLerp += Time.deltaTime / jumpDuration;
            float clampedT = Mathf.Clamp01(tLerp);

            Vector3 pos = Vector3.Lerp(start, end, clampedT);
            float arc = 4f * clampedT * (1f - clampedT);
            pos.y += arc * jumpHeight;
            pos.z = -5f;

            playerMarker.position = pos;

            float angle = 360f * clampedT;
            Quaternion dRot = Quaternion.Euler(0f, 0f, angle * -1);

            for (int i = 0; i < spinSet.Length; i++)
            {
                Transform tr = spinSet[i];
                if (tr == null) continue;

                tr.rotation = startRots[i] * dRot;
            }

            yield return null;
        }

        // Snap end
        _currentIndex = newIndex;
        Vector3 finalPos = end;
        finalPos.z = -5f;
        playerMarker.position = finalPos;

        // Reset spin targets while preserving facing
        for (int i = 0; i < spinSet.Length; i++)
        {
            Transform tr = spinSet[i];
            if (tr == null) continue;

            Vector3 e = tr.eulerAngles;
            tr.rotation = Quaternion.Euler(0f, e.y, 0f);
        }

        _isJumping = false;

        var tile = _tiles[_currentIndex];
        if (!tile.visited)
        {
            tile.visited = true;
            if (tile.def is IDungeonTileOnLand onLand)
                onLand.OnLand(this, _currentIndex);
        }
    }

    /// <summary>
    /// Snaps player to tile location (no animation)
    /// </summary>
    private void SnapPlayerTo(int index)
    {
        if (_tiles.Count == 0 || playerMarker == null) return;

        index = Mathf.Clamp(index, 0, _tiles.Count - 1);
        _currentIndex = index;

        Vector3 basePos = _tiles[index].transform.position;
        Vector3 pos = basePos + Vector3.up * idleYOffset;

        pos.z = -5f;

        playerMarker.position = pos;
    }

    private void ClearExistingTiles()
    {
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            var child = transform.GetChild(i);
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
