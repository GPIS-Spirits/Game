// Enum Definitions [ we can separate later ]

/// <summary>
/// Used for Chests, Equipment, Cards, etc
/// </summary>
public enum Quality
{
    Common,
    Uncommon,
    Rare,
    Epic,
    Legendary
}

/// <summary>
/// Used for Elemental types, Card damage, Equipment, etc
/// </summary>
public enum Element
{
    Fire,
    Water,
    Earth,
    Air,
    Light,
    Dark
}

/// <summary>
/// Used for choices player receives upon reaching Goal Tile
/// </summary>
public enum GoalChoices
{
    Exit,
    SameLevel,
    LowerLevel
}

public enum RestChoices
{
    Pass,
    Eat,
    Sleep,
    Encounter
}

/// <summary>
/// Actions for Cards { can adjust later }
/// </summary>
public enum CardActionType
{
    Attack,
    Defend,
    Special,
    Merge
}

/// <summary>
/// Type of Targeting for Card play
/// </summary>
public enum TargetingMode
{
    None,               // No target needed (self-only or global)
    Self,               // The player
    SingleEnemy,        // Select one enemy
    AllEnemies,         // Hits all enemies
    AllyAny,            // Select an allied elemental
    LowestHpEnemy,      // Auto-picks enemy with lowest HP (not selectable)
    Multiple,           // Allows multiple selection
    RandomEnemy         // Auto-picks a random enemy
}

/// <summary>
/// Card Effects / Tags
/// </summary>
public enum CardTag
{
    None,
    Burn,
    Stun,
    Heal,
    Shield,
    Cleanse,
    Shuffle,
    MultiHit,
    AOE,
    Multiple,
    ElementalSynergy
}

/// <summary>
/// Effects that can be applied
/// </summary>
public enum EffectType
{
    Damage,
    Shield,
    Heal,
    ApplyStatus,     // Burn, StunStack, etc.
    Cleanse,         // Remove Burn from player, etc.
    ShuffleEnemy,    // Shuffle formation; used by Air
    MultiHit,        // Repeats another effect N times
    Conditional,     // Gate an inner effect on a condition
    ModifyTurnMods   // Adjust turn-scoped modifiers (e.g., All Fire attack twice)
}