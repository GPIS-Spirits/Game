using UnityEngine;

public class ElementalSpawner : MonoBehaviour
{
    [Header("All Elemental Definitions")]
    public ElementalDef[] defs;

    [Header("Quality Roller")]
    public QualityRoller qualityRoller;

    [Header("Rarity Multipliers")]
    public float commonMult = 1.0f;
    public float uncommonMult = 1.15f;
    public float rareMult = 1.30f;
    public float epicMult = 1.65f;
    public float legendaryMult = 2.0f;

    // =====================================================================
    // RANDOM ACCESSORS (called externally if desired)
    // =====================================================================

    public Element GetRandomElement()
    {
        if (defs == null || defs.Length == 0)
        {
            Debug.LogError("No ElementalDefs assigned to ElementalSpawner.");
            return Element.Fire;
        }

        int idx = Random.Range(0, defs.Length);
        return defs[idx].type;
    }

    public Quality GetRandomQuality()
    {
        return qualityRoller.Roll();
    }

    // =====================================================================
    // SINGLE SPAWN FUNCTION
    // =====================================================================

    /// <summary>
    /// Spawns a specific Element + Quality at a Transform.
    /// Randomness happens externally if desired.
    /// </summary>
    public Elemental Spawn(Element element, Quality quality, Transform location)
    {
        ElementalDef def = GetDef(element);
        if (def == null)
        {
            Debug.LogError($"No ElementalDef found for element type {element}");
            return null;
        }

        // Parent the spawned elemental to the slot transform
        GameObject inst = Instantiate(def.prefab, location.position, location.rotation, location);

        Elemental e = inst.GetComponent<Elemental>();
        if (e == null)
        {
            Debug.LogError($"Prefab for element {element} does not contain an Elemental component.");
            return null;
        }

        // Assign data
        e.def = def;
        e.quality = quality;

        // Rename for clarity
        string uid = System.Guid.NewGuid().ToString("N").Substring(0, 4);
        inst.name = $"{element}_{quality}_{uid}";

        // Base stats
        e.hp = def.baseHP;
        e.dmg = def.baseDmg;
        e.armor = def.baseArmor;
        e.effectStrength = def.effectStrength;
        e.resists = new System.Collections.Generic.List<Resist>(def.resists);

        // Apply rarity scaling
        float mult = GetMultiplier(quality);
        MultiplyAllStats(e, mult);

        // Set color for Overlay
        ApplyRarityColor(inst, quality);
        return e;
    }


    // =====================================================================
    // INTERNAL HELPERS
    // =====================================================================

    private ElementalDef GetDef(Element type)
    {
        foreach (var d in defs)
            if (d.type == type)
                return d;
        return null;
    }

    private int MultiplyStat(int stat, float mult)
    {
        return Mathf.RoundToInt(stat * mult);
    }

    private void MultiplyAllStats(Elemental e, float mult)
    {
        e.hp = MultiplyStat(e.hp, mult);
        e.dmg = MultiplyStat(e.dmg, mult);
        e.armor = MultiplyStat(e.armor, mult);
        e.effectStrength = MultiplyStat(e.effectStrength, mult);
    }

    private float GetMultiplier(Quality q)
    {
        switch (q)
        {
            case Quality.Common: return commonMult;
            case Quality.Uncommon: return uncommonMult;
            case Quality.Rare: return rareMult;
            case Quality.Epic: return epicMult;
            case Quality.Legendary: return legendaryMult;
        }
        return 1f;
    }

    private Color GetRarityColor(Quality q)
    {
        switch (q)
        {
            case Quality.Common: return new Color(0.75f, 0.75f, 0.75f);         // Silver
            case Quality.Uncommon: return new Color(0.6f, 1.0f, 0.6f);          // Light Green
            case Quality.Rare: return new Color(0.4f, 0.6f, 1.0f);              // Blue
            case Quality.Epic: return new Color(0.65f, 0.3f, 0.9f);             // Purple
            case Quality.Legendary: return new Color(1.0f, 0.35f, 0.35f);       // Red
        }
        return Color.white;
    }
    private void ApplyRarityColor(GameObject inst, Quality quality)
    {
        Transform overlay = inst.transform.Find("RarityOverlay");
        if (overlay == null)
            return;

        SpriteRenderer sr = overlay.GetComponent<SpriteRenderer>();
        if (sr == null)
            return;

        sr.color = GetRarityColor(quality);
    }

}
