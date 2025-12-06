using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "_SO/Cards/PlayableCard")]
public class PlayableCardDef : BaseCardDef
{
    [Header("Card Settings")]
    public CardActionType actionType = CardActionType.Attack;
    public TargetingMode targetingMode = TargetingMode.SingleEnemy;
    public bool requiresTargetSelection = true;

    [Header("Effects")]
    public List<EffectDef> effects = new();
    public List<UpgradeOverride> upgradeOverrides = new();

    [System.Serializable]
    public class UpgradeOverride
    {
        [Min(1)] public int upgradeLevel = 1;
        [Tooltip("Replace the base Effects list at this upgrade level.")]
        public List<EffectDef> overrideEffects = new();
    }
}
