using UnityEngine;

[CreateAssetMenu(menuName = "_SO/Elementals/QualityRoller")]
public class QualityRoller : ScriptableObject
{
    [Header("Rarity Weights (sum does not need to be 1.0)")]
    public float common = 50f;
    public float uncommon = 30f;
    public float rare = 15f;
    public float epic = 4f;
    public float legendary = 1f;

    public Quality Roll()
    {
        float total =
            common + uncommon + rare + epic + legendary;

        float roll = Random.value * total;

        if (roll < common)
            return Quality.Common;

        roll -= common;
        if (roll < uncommon)
            return Quality.Uncommon;

        roll -= uncommon;
        if (roll < rare)
            return Quality.Rare;

        roll -= rare;
        if (roll < epic)
            return Quality.Epic;

        return Quality.Legendary;
    }
}
