using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayableCardUI : MonoBehaviour
{
    [Header("Data Reference")]
    [SerializeField] private CardData cardData;

    [Header("UI References")]
    [SerializeField] private TMP_Text HPText;
    [SerializeField] private TMP_Text DamageText;
    [SerializeField] private Image TypeImage;
    [SerializeField] private Image SpriteImage;
    [SerializeField] private TMP_Text DescriptionText;
    [SerializeField] private TMP_Text FlavorText;

    private void Start()
    {
        if (cardData != null)
            UpdateUI();
    }

    public void SetCardData(CardData data)
    {
        cardData = data;
        UpdateUI();
    }

    private void UpdateUI()
    {
        HPText.text = cardData.hp.ToString();
        DamageText.text = cardData.attack.ToString();
        TypeImage.sprite = cardData.typeImage;
        SpriteImage.sprite = cardData.spriteImage;
        DescriptionText.text = cardData.description;
        FlavorText.text = cardData.flavorText;
    }
}