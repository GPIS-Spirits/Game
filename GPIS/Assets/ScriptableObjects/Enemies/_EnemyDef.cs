using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "_SO/Enemies/Standard")]
public class EnemyDef : BaseNPCDef
{
    [Header("Combat Stats")]
    public int maxHealth;
    public int attack;
    public int defense;
    public int maxElementals;
    public int expReward;

    [Header("AI & Behavior")]
    public EnemyAIProfile aiProfile;

    // Enemies will have a list of Elementals, same as players, to fight with

    [Header("Elemental Pets")]
    public List<ElementalDef> elementals;
}