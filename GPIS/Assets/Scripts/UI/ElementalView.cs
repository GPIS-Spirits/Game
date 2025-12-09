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

    private Elemental selectedElem;
    public bool isOpen = false;

    private void Start()
    {
        detailsPanel.OnRenamed += OnElementalRenamed;
    }

    private void Update()
    {
        // block C while typing
        if (detailsPanel.renameInput.IsActive()) return;

        if (Input.GetKeyDown(KeyCode.C))
            ToggleView();
    }

    public void ToggleView()
    {
        isOpen = !isOpen;
        panel.SetActive(isOpen);

        detailsPanel.Show(null);

        if (isOpen)
            RefreshSlots();
    }

    // -----------------------------------------------------
    // Refresh slots (called when rename happens)
    // -----------------------------------------------------
    private void OnElementalRenamed()
    {
        RefreshSlots();
        if (selectedElem != null)
            detailsPanel.Show(selectedElem);
    }

    private void RefreshSlots()
    {
        detailsPanel.Show(null);

        List<Elemental> fly = player.GetFlyingList();
        List<Elemental> ground = player.GetGroundList();

        // FLYING
        for (int i = 0; i < flyingSlots.Length; i++)
        {
            if (i < fly.Count)
                PopulateSlot(flyingSlots[i], fly[i]);
            else
                flyingSlots[i].SetEmpty();
        }

        // GROUND
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

        // icon
        var bodySR = elem.def.prefab.GetComponentInChildren<SpriteRenderer>();
        if (bodySR != null)
        {
            slot.icon.sprite = bodySR.sprite;
            slot.icon.color = bodySR.color;
        }

        // outline
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

        // name
        slot.nameText.text = elem.gameObject.name;

        // click handler
        slot.button.onClick.RemoveAllListeners();
        slot.button.onClick.AddListener(() =>
        {
            selectedElem = elem;
            detailsPanel.Show(elem);
        });
    }
}
