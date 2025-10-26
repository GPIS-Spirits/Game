using UnityEngine;

public abstract class BaseCardDef : ScriptableObject
{
    [Header("Card Info")]
    public string cardId;
    public string displayName;
    [TextArea] public string description;
    public Sprite artwork;
    public GameObject cardViewPrefab;

    [Header("Meta")]
    public Element element = Element.Fire;
    public Quality rarity = Quality.Common;
    public CardTag[] tags;

    [Header("Upgrades")]
    public bool isUpgradeable = true;
    [Min(0)] public int maxUpgradeLevel = 2;
}
