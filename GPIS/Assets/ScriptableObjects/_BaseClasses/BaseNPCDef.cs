using UnityEngine;

public abstract class BaseNPCDef : ScriptableObject
{
    [Header("NPC Info")]
    public string npcID;
    public string npcName;
    public Sprite portrait; // different than prefab for artwork. small img
    public string description;
}
