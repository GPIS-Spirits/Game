using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Attached to Elementals at spawntime and calculates stats
/// </summary>
[DisallowMultipleComponent]
public class Elemental : MonoBehaviour
{
    [Header("Definition & Roll")]
    public ElementalDef def;
    public Quality quality;

    [Header("Final Stats")]
    public int hp;
    public int dmg;
    public int armor;
    public int effectStrength;

    [Header("Resists (copied from def)")]
    public List<Resist> resists = new();

    /// <summary>
    /// Easy debugging of elementals
    /// </summary>
    /// <returns></returns>
    public override string ToString()
    {
        var name = def != null ? def.npcName : "Elemental";
        return $"{name} [{quality}] HP:{hp} DMG:{dmg} ARM:{armor} FX:{effectStrength}";
    }
}
