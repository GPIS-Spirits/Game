using UnityEngine;

[CreateAssetMenu(fileName = "NewEffectDef", menuName = "Definitions/Effect")]
public class EffectDef : ScriptableObject
{
    public string effectName;
    public EffectType type;
    public int power;
    public int duration;

    [TextArea] public string description;
}

public enum EffectType
{
    Damage,
    Heal,
    ApplyStatus,
    DrawCard,
    GainEnergy,
    Shield
}
