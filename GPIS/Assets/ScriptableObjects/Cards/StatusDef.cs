using UnityEngine;

[CreateAssetMenu(fileName = "NewStatusDef", menuName = "Definitions/Status")]
public class StatusDef : ScriptableObject
{
    public string statusName;
    public StatusType statusType;
    public Sprite icon;
    public int baseDuration;

    [TextArea] public string description;

    public EffectDef onApplyEffect;
    public EffectDef onTurnStartEffect;
    public EffectDef onExpireEffect;
}

public enum StatusType
{
    Buff,
    Debuff
}
