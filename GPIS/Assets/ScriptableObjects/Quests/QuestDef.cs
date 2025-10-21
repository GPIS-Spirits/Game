// Quest SO Data Structure

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Quest", menuName = "QuestStats")]
public class QuestDef : ScriptableObject
{
    public string questName;
    public string questDesc;
    public int reward; // Placeholder for currency reward upon completing quest.
}
