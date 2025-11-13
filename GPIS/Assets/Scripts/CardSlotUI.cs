using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CardSlotUI : MonoBehaviour {
    public Image artwork;
    public TMP_Text costText;
    public Image elementIcon;
    public TMP_Text nameText;

    public void Populate(CardData data) {
        if (data == null) { Clear(); return; }
        artwork.sprite = data.artwork;
        costText.text = data.cost.ToString();
        nameText.text = data.cardName;
        elementIcon.sprite = ElementIconFor(data.element);
    }

    void Clear() {
        artwork.sprite = null;
        costText.text = "";
        nameText.text = "";
        elementIcon.sprite = null;
    }

    Sprite ElementIconFor(ElementType el) {
        // implement mapping; could be a ScriptableObject or static dictionary
        return ElementIconBank.Instance.GetIcon(el);
    }
}
