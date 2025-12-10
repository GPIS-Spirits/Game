using System.Collections.Generic;

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
    // INTERNAL LIST + CONSTRUCTOR
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

    public IReadOnlyList<Elemental> List => all;
    public int Count => all.Count;
}
