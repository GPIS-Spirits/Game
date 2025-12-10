using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ResourceUI : MonoBehaviour
{
    [Header("Hooked in Prefab")]
    public Image icon;
    public TMP_Text nameText;
    public TMP_Text countText;

    public Element element;
    public void Setup(Element elementType, Sprite iconSprite)
    {
        element = elementType;
        icon.sprite = iconSprite;
        nameText.text = elementType.ToString() + " Shard";
        countText.text = "";
    }

    // ---------------------------------------------------------
    // COUNT UPDATE (will be used later)
    // ---------------------------------------------------------
    public void UpdateCount(int amount)
    {
        countText.text = amount.ToString();
    }
}
