using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Static - Runs without scene attachment.
/// </summary>
public static class ElementalFactory
{
    /// <summary>
    /// Sets chance to get each quality
    /// </summary>
    private static readonly (Quality q, float w)[] Weights =
    {
        (Quality.Common,    0.45f),
        (Quality.Uncommon,  0.30f),
        (Quality.Rare,      0.14f),
        (Quality.Epic,      0.07f),
        (Quality.Legendary, 0.04f),
    };

    /// <summary>
    /// Sets the multipliers based on quality
    /// </summary>
    private static readonly Dictionary<Quality, float> Multipliers = new()
    {
        { Quality.Common,    1.00f },
        { Quality.Uncommon,  1.10f },
        { Quality.Rare,      1.25f },
        { Quality.Epic,      1.50f },
        { Quality.Legendary, 1.80f },
    };

    /// <summary>
    /// Calculates the cumulative distribution table only once
    /// Normalizes weights for sanity (if total value was 1.25, would be normaled to 1.0 max)
    /// </summary>
    private static readonly (Quality q, float cum)[] cumulativeDist;

    static ElementalFactory()
    {
        float total = 0f;
        cumulativeDist = new (Quality, float)[Weights.Length];
        for (int i = 0; i < Weights.Length; i++)
        {
            total += Weights[i].w;
            cumulativeDist[i] = (Weights[i].q, total);
        }

        
        if (!Mathf.Approximately(total, 1f))
        {
            for (int i = 0; i < cumulativeDist.Length; i++)
                cumulativeDist[i].cum /= total;
        }
    }

    /// <summary>
    /// Spawns an instance, rolls quality, scales stats, and returns the GameObject.
    /// </summary>
    public static GameObject Spawn(ElementalDef def, Vector3 position, Quaternion rotation = default, Transform parent = null)
    {
        if (def == null)
        {
            Debug.LogError("ElementalFactory.Spawn: def is null.");
            return null;
        }

        if (def.prefab == null)
        {
            Debug.LogError($"ElementalFactory.Spawn: def.prefab is null on {def.name}.");
            return null;
        }

        if (rotation == default) rotation = Quaternion.identity;

        var go = UnityEngine.Object.Instantiate(def.prefab, position, rotation, parent);
        var inst = go.AddComponent<Elemental>();

        inst.def = def;
        inst.quality = RollQuality();

        float mult = GetMultiplier(inst.quality);
        ApplyScaledStats(def, inst, mult);

        if (def.resists != null && def.resists.Count > 0)
            inst.resists = new List<Resist>(def.resists);
        else
            inst.resists.Clear();

        if (!string.IsNullOrEmpty(def.npcName))
            go.name = $"{def.npcName} [{inst.quality}]";

        return go;
    }

    private static void ApplyScaledStats(ElementalDef def, Elemental inst, float mult)
    {
        inst.hp = Mathf.Max(1, Mathf.RoundToInt(def.baseHP * mult));
        inst.dmg = Mathf.Max(0, Mathf.RoundToInt(def.baseDmg * mult));
        inst.armor = Mathf.Max(0, Mathf.RoundToInt(def.baseArmor * mult));
        inst.effectStrength = Mathf.Max(0, Mathf.RoundToInt(def.effectStrength * mult));
    }

    private static Quality RollQuality()
    {
        float r = UnityEngine.Random.value; // [0,1)
        for (int i = 0; i < cumulativeDist.Length; i++)
        {
            if (r < cumulativeDist[i].cum)
                return cumulativeDist[i].q;
        }
        return cumulativeDist[cumulativeDist.Length - 1].q;
    }

    private static float GetMultiplier(Quality q)
    {
        if (Multipliers.TryGetValue(q, out float mult))
            return mult;
        return 1f;
    }
}
