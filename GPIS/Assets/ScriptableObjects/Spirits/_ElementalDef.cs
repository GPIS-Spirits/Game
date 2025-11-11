using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;

[CreateAssetMenu(menuName = "_SO/Elementals/Elemental")]
public class ElementalDef : BaseNPCDef
{
    [Header("Elemental Attributes")]
    public Element type = Element.Fire;
    public Quality quality = Quality.Common;            // sets multiplier to all stats
    public TargetingMode targeting = TargetingMode.SingleEnemy;

    [Header("Prefab")]
    public GameObject prefab;

    [Header("Stats")]
    public int baseHP;                                  // adds to players
    public int baseDmg;                                 // for spirit attacks
    public int baseArmor;                               // adds to players
    public List<Resist> resists;                        // each added to players
}
