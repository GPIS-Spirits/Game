using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DungeonPopupUI : MonoBehaviour {
    public GameObject popupPanel;
    public TMP_Text titleText;
    public TMP_Text descText;
    public Button confirmButton;
    public Button cancelButton;

    void Start() {
        // Hide popup by default
        popupPanel.SetActive(false);
        // Add listeners
        confirmButton.onClick.AddListener(OnConfirm);
        cancelButton.onClick.AddListener(ClosePopup);
    }

    public void ShowPopup(string title, string desc) {
        popupPanel.SetActive(true);
        titleText.text = title;
        descText.text = desc;
    }

    void OnConfirm() {
        Debug.Log("Confirmed!");
        ClosePopup();
    }

    public void ClosePopup() {
        popupPanel.SetActive(false);
    }
}
