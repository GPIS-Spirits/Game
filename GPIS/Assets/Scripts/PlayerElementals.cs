using System.Collections.Generic;
using UnityEngine;

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
        // EARTH = ground, other elements = flying
        bool isGround = (element == Element.Earth);

        List<Elemental> list = isGround ? groundElementals : flyingElementals;
        Transform[] slots = isGround ? groundPositions : flyingPositions;

        if (list.Count >= MAX)
            return;

        int slotIndex = list.Count; // always next available compact slot

        Elemental elem = spawner.Spawn(element, quality, slots[slotIndex]);
        list.Add(elem);
    }

    // ----------------------------------------------------------------------
    // REMOVE RANDOM
    // ----------------------------------------------------------------------

    public void RemoveRandomElemental()
    {
        int total = flyingElementals.Count + groundElementals.Count;
        if (total == 0)
            return;

        // Pick any list
        bool removeFlying = Random.value < (flyingElementals.Count / (float)total);

        List<Elemental> list = removeFlying ? flyingElementals : groundElementals;
        Transform[] slots = removeFlying ? flyingPositions : groundPositions;

        if (list.Count == 0)
            list = removeFlying ? groundElementals : flyingElementals;

        // Remove random within that list
        int idx = Random.Range(0, list.Count);
        Elemental toRemove = list[idx];
        Destroy(toRemove.gameObject);

        list.RemoveAt(idx);
        CompactList(list, slots);
    }

    // ----------------------------------------------------------------------
    // COMPACT LIST AND REPOSITION
    // ----------------------------------------------------------------------

    private void CompactList(List<Elemental> list, Transform[] slots)
    {
        for (int i = 0; i < list.Count; i++)
        {
            if (list[i] == null) continue;

            // Reparent to the correct slot
            list[i].transform.SetParent(slots[i]);

            // Reset position & rotation to slot
            list[i].transform.localPosition = Vector3.zero;
            list[i].transform.localRotation = Quaternion.identity;
        }
    }

}
