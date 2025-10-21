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