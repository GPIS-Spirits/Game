using UnityEngine;

[CreateAssetMenu(fileName = "NewEnemyAIProfile", menuName = "Definitions/EnemyAIProfile")]
public class EnemyAIProfile : ScriptableObject
{
    public string profileName;
    public AIPatternType aiPattern;
    public float aggressionLevel; // 0-1 scale
    public float defenseBias;     // 0-1 scale
    public float randomFactor;    // randomness in decisions

    [TextArea] public string description;
}

public enum AIPatternType
{
    Aggressive,
    Defensive,
    Balanced,
    Random,
    BossPattern
}
