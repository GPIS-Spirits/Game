using UnityEngine;

[CreateAssetMenu(fileName = "NewCardData", menuName = "Card Game/Card Data")]
public class CardData : ScriptableObject
{
    [Header("Card Info")]
    public string cardName;
    public int hp;
    public int attack;
    public Sprite typeImage;
    public Sprite spriteImage;
    
    [TextArea(2, 4)]
    public string description;
    
    [TextArea(2, 4)]
    public string flavorText;
}