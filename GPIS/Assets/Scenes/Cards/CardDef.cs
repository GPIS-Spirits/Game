using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "NewCardDef", menuName = "Definitions/Card")]
public class CardDef : ScriptableObject
{
    public string cardName;
    public Sprite artwork;
    public ElementType element;
    public CardType cardType;
    public int energyCost;

    [TextArea] public string description;

    public List<EffectDef> effects;
    public int upgradeLevel;
    public bool isUpgradeable;
}

public enum ElementType
{
    Fire, Water, Air, Earth, Shadow, Light, Steam, Lightning, Nature, Void
}

public enum CardType
{
    Spirit, Fusion, Passive, Ultimate
}
