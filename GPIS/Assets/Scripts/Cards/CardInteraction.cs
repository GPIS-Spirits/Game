// Attached to playable cards to define their actions & functionality based on "PlayableCardDef".

using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(PlayableCardDisplay))] // Assures that this can only be put on Game Objects with the "PlayableCardDisplay" script attached:
public class CardInteraction : MonoBehaviour
{
    [Header("Game Object Wiring")]
    public ElementalCombat ownerElemental; // Elemental that "owns" this card (Attack, Special Ability, etc.).
    public Image selectionHighlight;       // UI Image used to show a card is selected.

    public bool IsSelected { get; private set; } // "Auto-Implemented Get/Set" - C# shortcut that allows "IsSelected" to be accessed publicly, set privately.

    public PlayableCardDisplay Display { get; private set; } // Auot-Implemented Get/Set

    void Awake()
    {
        Display = GetComponent<PlayableCardDisplay>();
        SetSelected(false);
    }

    void Start()
    {
        var handManager = GetComponentInParent<HandManager>(); // Grabs 'HandManager' Parent of this card:
        if (handManager) handManager.AddToHand(this);          // Adds this card to the List of 'handManager' if present:
        else Debug.LogWarning("CardInteraction: No HandManager found in parents.");
    }

    public void OnCardClicked()
    {
        SetSelected(!IsSelected); // Call 'SetSelected' and pass it the opposite value of 'IsSelected':
    }

    // Enables/Disables card selection from the hand.
    public void SetSelected (bool selected)
    {
        IsSelected = selected;
        if (selectionHighlight) selectionHighlight.enabled = selected; // Enables 'selectionHighlight' if the card it is attached to is selected:
    }

    // Ensures 'this' card object is properly removed from the 'HandManager' List.
    void OnDestroy()
    {
        var handManager = GetComponentInParent<HandManager>();
        if (handManager) handManager.RemoveFromHand(this);
    }
}
