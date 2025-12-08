using System.Collections.Generic;
using UnityEngine;

public class ElementalView : MonoBehaviour
{
    [Header("References")]
    public PlayerElementals player;
    public GameObject panel;

    public ElementalSlotUI[] flyingSlots = new ElementalSlotUI[4];
    public ElementalSlotUI[] groundSlots = new ElementalSlotUI[4];

    public ElementalDetailsPanel detailsPanel;

    private bool isOpen = false;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
            ToggleView();
    }

    public void ToggleView()
    {
        isOpen = !isOpen;
        panel.SetActive(isOpen);

        if (isOpen)
            RefreshSlots();
    }

    // -----------------------------------------------------
    // Refresh the left-hand 8 slots
    // -----------------------------------------------------
    private void RefreshSlots()
    {
        // Clear details panel
        detailsPanel.Show(null);

        List<Elemental> fly = player.GetFlyingList();
        List<Elemental> ground = player.GetGroundList();

        // FLYING SLOTS
        for (int i = 0; i < flyingSlots.Length; i++)
        {
            if (i < fly.Count)
                PopulateSlot(flyingSlots[i], fly[i]);
            else
                flyingSlots[i].SetEmpty();
        }

        // GROUND SLOTS
        for (int i = 0; i < groundSlots.Length; i++)
        {
            if (i < ground.Count)
                PopulateSlot(groundSlots[i], ground[i]);
            else
                groundSlots[i].SetEmpty();
        }
    }

    private void PopulateSlot(ElementalSlotUI slot, Elemental elem)
    {
        slot.SetElement(elem);

        // Set icon
        var bodySR = elem.def.prefab.GetComponentInChildren<SpriteRenderer>();
        if (bodySR != null)
        {
            slot.icon.sprite = bodySR.sprite;
            slot.icon.color = bodySR.color;
        }

        // Set outline (rarity overlay)
        var overlay = elem.transform.Find("RarityOverlay");
        if (overlay != null)
        {
            var sr = overlay.GetComponent<SpriteRenderer>();
            if (sr != null)
            {
                slot.outline.sprite = sr.sprite;
                slot.outline.color = sr.color;
            }
        }

        slot.nameText.text = $"{elem.def.type}";

        // Hook up click event
        slot.button.onClick.RemoveAllListeners();
        slot.button.onClick.AddListener(() =>
        {
            detailsPanel.Show(elem);
        });
    }
}
