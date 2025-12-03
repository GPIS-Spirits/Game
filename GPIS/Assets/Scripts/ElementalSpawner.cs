using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("Spawners/Elemental Spawner (Slots Only)")]
public class ElementalSpawner : MonoBehaviour
{
    [Header("Elemental Sources")]
    public List<ElementalDef> candidates = new();

    [Header("Slots (order = element0..element3)")]
    public Transform[] flyingSlots; // Slot_001..Slot_004 (air)
    public Transform[] groundSlots; // Slot_001..Slot_004 (ground)

    [Header("Limits")]
    [Min(0)] public int initialSpawn = 0;
    [Min(1)] public int maxAlive = 4;

    [Header("Placement")]
    public bool snapToSlotLocalZero = true;

    // runtime
    private readonly List<Elemental> _alive = new();
    private int _nextFly = 0;
    private int _nextGround = 0;

    public IReadOnlyList<Elemental> Alive => _alive;

    void Start()
    {
        if (initialSpawn > 0) SpawnInitial();
    }

    [ContextMenu("Spawn Initial")]
    public void SpawnInitial()
    {
        if (candidates == null || candidates.Count == 0) return;
        int target = Mathf.Clamp(initialSpawn, 0, maxAlive);
        int guard = 64;

        while (_alive.Count < target && guard-- > 0)
        {
            var def = candidates[Random.Range(0, candidates.Count)];
            if (def == null) continue;
            if (Spawn(def) == null) continue;
        }
    }

    [ContextMenu("Spawn Random")]
    public void SpawnRandom()
    {
        if (candidates == null || candidates.Count == 0) return;
        var def = candidates[Random.Range(0, candidates.Count)];
        Spawn(def);
    }

    public Elemental Spawn(ElementalDef def)
    {
        if (def == null) return null;
        PruneNulls();
        if (_alive.Count >= maxAlive) return null;

        bool isEarth = def.type == Element.Earth;
        var slots = isEarth ? groundSlots : flyingSlots;
        if (slots == null || slots.Length == 0) return null;

        // find next free slot in sequence, wrap, skip occupied
        int start = isEarth ? _nextGround : _nextFly;
        Transform slot = null;

        for (int i = 0; i < slots.Length; i++)
        {
            int idx = (start + i) % slots.Length;
            var s = slots[idx];
            if (!s) continue;

            // one occupant per slot: childCount==0 means free
            if (s.childCount == 0)
            {
                slot = s;
                if (isEarth) _nextGround = (idx + 1) % slots.Length;
                else _nextFly = (idx + 1) % slots.Length;
                break;
            }
        }
        if (slot == null) return null;

        // instantiate as child of the slot
        var go = ElementalFactory.Spawn(def, slot.position, slot.rotation, slot);
        if (!go) return null;

        if (snapToSlotLocalZero)
        {
            var tr = go.transform;
            tr.localPosition = Vector3.zero;
            tr.localRotation = Quaternion.identity;
        }

        var inst = go.GetComponent<Elemental>();
        if (!inst) return null;

        _alive.Add(inst);
        return inst;
    }

    public void Despawn(Elemental inst)
    {
        if (!inst) return;
        _alive.Remove(inst);
        Destroy(inst.gameObject);
    }

    public void DespawnAll()
    {
        for (int i = 0; i < _alive.Count; i++)
            if (_alive[i]) Destroy(_alive[i].gameObject);
        _alive.Clear();
    }

    public void SetMaxAlive(int newMax)
    {
        maxAlive = Mathf.Max(1, newMax);
        PruneNulls();
        while (_alive.Count > maxAlive)
        {
            var last = _alive[_alive.Count - 1];
            Despawn(last);
        }
    }

    private void PruneNulls()
    {
        for (int i = _alive.Count - 1; i >= 0; --i)
            if (!_alive[i]) _alive.RemoveAt(i);
    }
}
