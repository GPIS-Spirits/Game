using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class ElementalDetailsPanel : MonoBehaviour
{
    public TMP_Text nameText;
    public TMP_Text qualityText;
    public Image icon;
    public Image outline;
    public TMP_Text statsText;
    public TMP_Text resistText;

    [Header("Rename UI")]
    public Button renameButton;
    public TMP_InputField renameInput;

    // event used to notify ElementalView to refresh
    public Action OnRenamed;

    public Elemental currentElem;
    public ElementalCollectionDisplay listView;

    public void Show(Elemental elem)
    {
        currentElem = elem;

        if (elem == null)
        {
            nameText.text = "";
            qualityText.text = "";
            statsText.text = "";
            resistText.text = "";

            icon.sprite = null;
            icon.color = Color.clear;

            outline.sprite = null;
            outline.color = Color.clear;
            outline.enabled = false;

            renameButton.interactable = false;
            renameButton.gameObject.SetActive(false);
            renameInput.gameObject.SetActive(false);
            return;
        }

        // Show rename button
        renameButton.interactable = true;
        renameButton.gameObject.SetActive(true);
        renameInput.gameObject.SetActive(false);

        // Name (actual generated name)
        nameText.text = elem.gameObject.name;

        // Quality
        qualityText.text = elem.quality.ToString();

        // Icon Sprite
        SpriteRenderer bodySR = elem.def.prefab.GetComponentInChildren<SpriteRenderer>();
        if (bodySR != null)
        {
            icon.sprite = bodySR.sprite;
            icon.color = bodySR.color;
        }

        // Rarity overlay from instance
        Transform overlayTransform = elem.transform.Find("RarityOverlay");
        if (overlayTransform != null)
        {
            var overlaySR = overlayTransform.GetComponent<SpriteRenderer>();
            if (overlaySR != null)
            {
                outline.enabled = true;
                outline.sprite = overlaySR.sprite;
                outline.color = overlaySR.color;
            }
        }

        // Stats
        statsText.text =
            $"HP: {elem.hp}\nARM: {elem.armor}\nDMG: {elem.dmg}\nFX: {elem.effectStrength}\n";

        // Resists
        if (elem.resists != null && elem.resists.Count > 0)
        {
            resistText.text = "";
            foreach (var r in elem.resists)
                resistText.text += $"{r.element}: {r.percent}%\n";
        }
        else
        {
            resistText.text = "No Resists";
        }
    }

    // ------------------------------------------------------------
    // RENAME LOGIC
    // ------------------------------------------------------------

    public void BeginRename()
    {
        if (currentElem == null) return;

        renameInput.text = currentElem.gameObject.name;
        renameInput.gameObject.SetActive(true);
        renameInput.Select();
        renameInput.ActivateInputField();
    }

    public void ApplyRename()
    {
        if (currentElem == null) return;

        string newName = renameInput.text.Trim();
        if (!string.IsNullOrEmpty(newName))
            currentElem.name = newName;

        renameInput.gameObject.SetActive(false);

        OnRenamed?.Invoke();
        currentElem.NotifyRenamed();
        listView.RefreshList();
    }
    public void GetName()
    {
        if (currentElem == null)
            return;

        renameInput.text = currentElem.gameObject.name;
    }
}
