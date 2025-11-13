using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SidebarStats : MonoBehaviour {
    [Header("Stats")]
    public Slider hpSlider;
    public TMP_Text hpText;
    public Slider manaSlider;
    public TMP_Text manaText;

    public void SetHP(int current, int max) {
        hpSlider.maxValue = max;
        hpSlider.value = Mathf.Clamp(current, 0, max);
        hpText.text = $"{current} / {max}";
    }

    public void SetMana(int current, int max) {
        manaSlider.maxValue = max;
        manaSlider.value = Mathf.Clamp(current, 0, max);
        manaText.text = $"{current} / {max}";
    }
}