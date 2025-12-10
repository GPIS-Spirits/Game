using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ElementalCollectionDisplayItem : MonoBehaviour
{
    public Image icon;
    public Image outline;
    public TMP_Text nameText;
    public TMP_Text rarityText;
    public Button button;

    [HideInInspector] public Elemental referencedElement;

    public void SetEmpty()
    {
        referencedElement = null;

        nameText.text = "";
        rarityText.text = "";

        icon.sprite = null;
        outline.sprite = null;

        icon.color = Color.clear;
        outline.color = Color.clear;

        icon.enabled = false;
        outline.enabled = false;
        button.interactable = false;
    }

    public void SetElement(Elemental elem)
    {
        referencedElement = elem;
        button.interactable = true;

        icon.enabled = true;
        outline.enabled = true;

        SpriteRenderer sr = elem.GetComponentInChildren<SpriteRenderer>();
        if (sr != null)
        {
            icon.sprite = sr.sprite;
            icon.color = Color.white;
        }
        else
        {
            icon.sprite = null;
            icon.color = Color.clear;
        }

        nameText.text = elem.name;
        rarityText.text = elem.quality.ToString();
        outline.color = GetRarityColor(elem.quality);
    }

    Color GetRarityColor(Quality q)
    {
        switch (q)
        {
            case Quality.Common: return new Color(0.75f, 0.75f, 0.75f);
            case Quality.Uncommon: return new Color(0.6f, 1.0f, 0.6f);
            case Quality.Rare: return new Color(0.4f, 0.6f, 1.0f);
            case Quality.Epic: return new Color(0.65f, 0.3f, 0.9f);
            case Quality.Legendary: return new Color(1.0f, 0.35f, 0.35f);
        }
        return Color.white;
    }
}
