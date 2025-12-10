using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ElementalSlotUI : MonoBehaviour
{
    public Image icon;
    public Image outline;
    public TMP_Text nameText;
    public Button button;

    [HideInInspector] public Elemental referencedElement;

    public void SetEmpty()
    {
        referencedElement = null;
        nameText.text = "";
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
    }
}
