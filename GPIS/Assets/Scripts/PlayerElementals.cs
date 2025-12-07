using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct PlayerStats
{
    public int totalHP;
    public int totalArmor;
    public int totalDmg;
    public int totalEffect;

    public Dictionary<Element, int> resists;
    public override string ToString()
    {
        string line1 =
            $"STATS || HP: {totalHP} || ARM: {totalArmor} || DMG: {totalDmg} || FX: {totalEffect}";
        string line2 = "RESISTS";

        if (resists != null && resists.Count > 0)
        {
            foreach (var kvp in resists)
                line2 += $" || {kvp.Key}: {kvp.Value}%";
        }
        else
        {
            line2 += " || None";
        }

        return line1 + "\n" + line2;
    }
}

public class PlayerElementals : MonoBehaviour
{
    [Header("Spawner")]
    public ElementalSpawner spawner;

    [Header("Positions")]
    public Transform[] flyingPositions = new Transform[4];
    public Transform[] groundPositions = new Transform[4];

    private List<Elemental> flyingElementals = new List<Elemental>();
    private List<Elemental> groundElementals = new List<Elemental>();

    private const int MAX = 4;
    public PlayerStats stats;
    private void Start()
    {
        stats = RefreshStats();
        Debug.Log(stats);
    }
    void Update()
    {
        // R = spawn random elemental
        if (Input.GetKeyDown(KeyCode.R))
        {
            Element randomElement = spawner.GetRandomElement();
            Quality randomQuality = spawner.GetRandomQuality();
            SpawnElemental(randomElement, randomQuality);
        }

        // T = remove random elemental
        if (Input.GetKeyDown(KeyCode.T))
        {
            RemoveRandomElemental();
        }
    }

    // ----------------------------------------------------------------------
    // SPAWN
    // ----------------------------------------------------------------------

    public void SpawnElemental(Element element, Quality quality)
    {
        bool isGround = (element == Element.Earth);

        List<Elemental> list = isGround ? groundElementals : flyingElementals;
        Transform[] slots = isGround ? groundPositions : flyingPositions;

        if (list.Count >= MAX)
            return;

        int slotIndex = list.Count;

        Elemental elem = spawner.Spawn(element, quality, slots[slotIndex]);
        list.Add(elem);

        Debug.Log($"Spawned Elemental: {element} ({quality}) into slot {slotIndex}");

        stats = RefreshStats();
        Debug.Log(stats);
    }


    // ----------------------------------------------------------------------
    // REMOVE RANDOM
    // ----------------------------------------------------------------------

    public void RemoveRandomElemental()
    {
        int total = flyingElementals.Count + groundElementals.Count;
        if (total == 0)
            return;

        bool removeFlying = Random.value < (flyingElementals.Count / (float)total);

        List<Elemental> list = removeFlying ? flyingElementals : groundElementals;
        Transform[] slots = removeFlying ? flyingPositions : groundPositions;

        if (list.Count == 0)
            list = removeFlying ? groundElementals : flyingElementals;

        int idx = Random.Range(0, list.Count);
        Elemental toRemove = list[idx];

        Debug.Log($"Removed Elemental: {toRemove.def.type} ({toRemove.quality}) from slot {idx}");

        Destroy(toRemove.gameObject);

        list.RemoveAt(idx);
        CompactList(list, slots);

        stats = RefreshStats();
        Debug.Log(stats);
    }


    // ----------------------------------------------------------------------
    // COMPACT LIST AND REPOSITION
    // ----------------------------------------------------------------------

    private void CompactList(List<Elemental> list, Transform[] slots)
    {
        for (int i = 0; i < list.Count; i++)
        {
            if (list[i] == null) continue;

            // Reparent
            list[i].transform.SetParent(slots[i]);

            // Adjust Positions
            list[i].transform.localPosition = Vector3.zero;
            list[i].transform.localRotation = Quaternion.identity;
        }
    }
    public PlayerStats RefreshStats()
    {
        PlayerStats stats = new PlayerStats
        {
            totalHP = 0,
            totalArmor = 0,
            totalDmg = 0,
            totalEffect = 0,
            resists = new Dictionary<Element, int>()
        };

        foreach (var elemList in new[] { flyingElementals, groundElementals })
        {
            foreach (var elem in elemList)
            {
                if (elem == null)
                    continue;

                // Basic stats
                stats.totalHP += elem.hp;
                stats.totalArmor += elem.armor;

                // These won't be used but will be calculated anyway
                stats.totalDmg += elem.dmg;
                stats.totalEffect += elem.effectStrength;

                // Resists
                foreach (var r in elem.resists)
                {
                    if (!stats.resists.ContainsKey(r.element))
                        stats.resists[r.element] = 0;

                    stats.resists[r.element] += r.percent;
                }
            }
        }

        return stats;
    }

}
