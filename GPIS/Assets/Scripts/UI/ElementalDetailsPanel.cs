using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ElementalDetailsPanel : MonoBehaviour
{
    public TMP_Text nameText;
    public Image icon;
    public Image outline;
    public TMP_Text statsText;
    public TMP_Text resistText;

    public void Show(Elemental elem)
    {
        if (elem == null)
        {
            nameText.text = "";
            statsText.text = "";
            resistText.text = "";
            icon.sprite = null;
            outline.sprite = null;
            return;
        }

        // Name
        nameText.text = $"{elem.def.type} [{elem.quality}]";

        // Extract base sprite
        SpriteRenderer bodySR = elem.def.prefab.GetComponentInChildren<SpriteRenderer>();
        if (bodySR != null)
        {
            icon.sprite = bodySR.sprite;
            icon.color = bodySR.color;
        }

        // Extract overlay from instance
        Transform overlayTransform = elem.transform.Find("RarityOverlay");
        if (overlayTransform != null)
        {
            var overlaySR = overlayTransform.GetComponent<SpriteRenderer>();
            if (overlaySR != null)
            {
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
}
