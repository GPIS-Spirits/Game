using System.Collections.Generic;
using UnityEngine;

public class ElementalCollection
{
    // ============================================================
    // SORT ORDER
    // ============================================================
    private static readonly Dictionary<Element, int> ElementOrder = new()
    {
        { Element.Earth, 0 },
        { Element.Water, 1 },
        { Element.Fire, 2 },
        { Element.Air,   3 },
        { Element.Light, 4 },
        { Element.Dark,  5 }
    };

    private static readonly Dictionary<Quality, int> QualityOrder = new()
    {
        { Quality.Legendary, 0 },
        { Quality.Epic,      1 },
        { Quality.Rare,      2 },
        { Quality.Uncommon,  3 },
        { Quality.Common,    4 }
    };

    // ============================================================
    // INTERNAL LIST
    // ============================================================
    private readonly List<Elemental> all = new();

    public ElementalCollection()
    {
    }

    // ============================================================
    // ADD / REMOVE / SORT
    // ============================================================
    public void Add(Elemental elem)
    {
        if (elem == null) return;
        all.Add(elem); 
        elem.OnRenamed += HandleRename;
        Sort();
    }

    public bool Remove(Elemental elem)
    {
        if (elem == null) return false;

        bool removed = all.Remove(elem);
        if (removed)
            Sort();
        return removed;
    }

    private void Sort()
    {
        all.Sort((a, b) =>
        {
            int typeA = ElementOrder[a.def.type];
            int typeB = ElementOrder[b.def.type];

            int typeCmp = typeA.CompareTo(typeB);
            if (typeCmp != 0)
                return typeCmp;

            int rarityA = QualityOrder[a.quality];
            int rarityB = QualityOrder[b.quality];
            return rarityA.CompareTo(rarityB);
        });
    }

    // ============================================================
    // USED FOR DEBUGGING
    // ============================================================
    public Elemental AddRandomTestCopy()
    {
        if (all.Count == 0)
            return null;

        int randIndex = UnityEngine.Random.Range(0, all.Count);
        Elemental src = all[randIndex];
        if (src == null)
            return null;

        GameObject go = new GameObject($"{src.def.type}_{src.quality}_TEST");
        Elemental clone = go.AddComponent<Elemental>();

        clone.def = src.def;
        clone.quality = src.quality;
        clone.hp = src.hp;
        clone.dmg = src.dmg;
        clone.armor = src.armor;
        clone.effectStrength = src.effectStrength;
        clone.resists = new List<Resist>(src.resists);

        string uid = System.Guid.NewGuid().ToString("N").Substring(0, 4);
        clone.name = $"{clone.def.type}_{clone.quality}_TEST_{uid}";

        all.Add(clone);
        clone.OnRenamed += HandleRename;    

        Sort();

        Debug.Log($"[TEST] Spawned Elemental: {clone.def.type} ({clone.quality}) into collection index {all.IndexOf(clone)}");

        return clone;
    }

    private void HandleRename(Elemental elem)
    {
        Sort();
    }

    public IReadOnlyList<Elemental> List => all;
    public int Count => all.Count;
}
