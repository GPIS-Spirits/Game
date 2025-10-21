using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "NewEnemyDef", menuName = "Definitions/Enemy")]
public class EnemyDef : ScriptableObject
{
    public string enemyName;
    public Sprite artwork;
    public ElementType element;
    public int maxHealth;
    public int attackPower;
    public int defense;
    public int expReward;

    public List<CardDef> possibleCards;
    public EnemyAIProfile aiProfile;
}
