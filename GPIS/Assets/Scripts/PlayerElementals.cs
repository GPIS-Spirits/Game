using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

[System.Serializable]
public struct PlayerStats
{
    public int totalHP;
    public int totalArmor;
    public int totalDmg;
    public int totalEffect;
    public int totalEnergy;

    public Dictionary<Element, int> resists;
    public override string ToString()
    {
        string line1 =
            $"STATS || HP: {totalHP} || ARM: {totalArmor} || DMG: {totalDmg} || FX: {totalEffect} || EN: {totalEnergy}";

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

    // Used to prevent typing issues while renaming
    public ElementalView elementalView;

    public int maxElementals = 4;
    private int currElementals = 0;

    [Header("Spawner")]
    public ElementalSpawner spawner;

    [Header("Positions")]
    public Transform[] flyingPositions = new Transform[4];
    public Transform[] groundPositions = new Transform[4];

    private List<Elemental> flyingElementals = new List<Elemental>();
    private List<Elemental> groundElementals = new List<Elemental>();


    [Header("Base Player Stats")]
    public int baseHP = 50;
    public int baseArmor = 0;
    public int baseDmg = 10;
    public int baseEffect = 0;
    public int baseEnergy = 3;

    public ElementalCollection collection;
    public ElementalCollectionDisplay listView;

    public PlayerStats stats;
    private void Awake()
    {
        collection = new ElementalCollection();
    }

    private void Start()
    {
        stats = RefreshStats();
        Debug.Log(stats);
    }
    void Update()
    {
        if (elementalView.isOpen == true) return;
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
    // SPAWNING FUNCTIONS
    // ----------------------------------------------------------------------

    // Used primarily for debugging
    public void SpawnElemental(Element element, Quality quality)
    {
        if (currElementals >= maxElementals)
        {
            Debug.Log("Too many elementals, not spawning.");
            return;
        }
        bool isGround = (element == Element.Earth);

        List<Elemental> list = isGround ? groundElementals : flyingElementals;
        Transform[] slots = isGround ? groundPositions : flyingPositions;

        if (list.Count >= maxElementals)
            return;

        int slotIndex = list.Count;

        Elemental elem = spawner.Spawn(element, quality, slots[slotIndex]);
        list.Add(elem);

        Debug.Log($"Spawned Elemental: {element} ({quality}) into slot {slotIndex}");
        ++currElementals;
        stats = RefreshStats();
        Debug.Log(stats);
        if (collection != null)
            collection.Add(elem);
        if (listView != null)
            listView.RefreshList();
    }

    // Used primarily for debugging
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
        --currElementals;
        list.RemoveAt(idx);
        CompactList(list, slots);

        stats = RefreshStats();
        Debug.Log(stats); 
        if (collection != null)
            collection.Remove(toRemove); 
        if (listView != null)
            listView.RefreshList();
    }

    public void RemoveElementalByName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            return;

        // Search flying list
        for (int i = 0; i < flyingElementals.Count; i++)
        {
            if (flyingElementals[i] != null &&
                flyingElementals[i].name == name)
            {
                var elem = flyingElementals[i];
                flyingElementals.RemoveAt(i);
                if (collection != null) collection?.Remove(elem);
                if (listView != null) listView.RefreshList();
                Destroy(elem.gameObject);
                currElementals--;
                CompactList(flyingElementals, flyingPositions);
                stats = RefreshStats();
                return;
            }
        }

        // Search ground list
        for (int i = 0; i < groundElementals.Count; i++)
        {
            if (groundElementals[i] != null &&
                groundElementals[i].name == name)
            {
                var elem = groundElementals[i];
                groundElementals.RemoveAt(i); 
                if (collection != null) collection?.Remove(elem);
                if (listView != null) listView.RefreshList();
                Destroy(elem.gameObject);
                currElementals--;
                CompactList(groundElementals, groundPositions);
                stats = RefreshStats();
                return;
            }
        }

        Debug.Log("No elemental found with name: " + name);
    }

    public Elemental SpawnFromCollection(Elemental sourceElem)
    {
        if (sourceElem == null)
        {
            Debug.LogError("SpawnFromCollection called with null Elemental.");
            return null;
        }

        if (currElementals >= maxElementals)
        {
            Debug.Log("Too many elementals, not spawning.");
            return null;
        }

        bool isGround = (sourceElem.def.type == Element.Earth);

        List<Elemental> list = isGround ? groundElementals : flyingElementals;
        Transform[] slots = isGround ? groundPositions : flyingPositions;

        if (list.Count >= maxElementals)
            return null;

        int slotIndex = list.Count;

        // Spawn using your existing spawner logic
        Elemental newElem = spawner.Spawn(
            sourceElem.def.type,
            sourceElem.quality,
            slots[slotIndex]
        );

        if (newElem == null)
            return null;

        newElem.hp = sourceElem.hp;
        newElem.dmg = sourceElem.dmg;
        newElem.armor = sourceElem.armor;
        newElem.effectStrength = sourceElem.effectStrength;
        newElem.resists = new List<Resist>(sourceElem.resists);

        list.Add(newElem);
        currElementals++;
        stats = RefreshStats();

        Debug.Log($"Spawned new Elemental from collection: {newElem.def.type} ({newElem.quality}) into slot {slotIndex}");

        return newElem;
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

    // ----------------------------------------------------------------------
    // USED TO UPDATE STATS
    // ----------------------------------------------------------------------

    public PlayerStats RefreshStats()
    {
        PlayerStats stats = new PlayerStats
        {
            totalHP = baseHP,
            totalArmor = baseArmor,
            totalDmg = baseDmg,
            totalEffect = baseEffect,
            totalEnergy = baseEnergy,
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

    // ----------------------------------------------------------------------
    // RETURNS FLYING AND GROUND LISTS FOR INV DISPLAY
    // ----------------------------------------------------------------------

    public List<Elemental> GetFlyingList() => flyingElementals;
    public List<Elemental> GetGroundList() => groundElementals;
}
