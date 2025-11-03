// Manager for accessing/manipulating cards in the Player's hand during combat.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandManager : MonoBehaviour
{
    [Header("Card UI Prefab")]
    [SerializeField] private GameObject cardPrefab; // Assigns Card UI Prefab for hand construction.

    [Header("Owner Elemental (For new card)")]
    [SerializeField] private ElementalCombat defaultOwnerElemental;

    public readonly List<CardInteraction> Hand = new(); // Dynamically Allocated List to hold all cards in the Player's hand (Explicitly set to 'readonly' mode).

    // Use Predefined List Operations to add passed card to the Player's hand.
    public void AddToHand(CardInteraction card)
    {
        if (!Hand.Contains(card)) Hand.Add(card);
    }

    // Use Predefined List Operations to remove passed card from the Player's hand.
    public void RemoveFromHand(CardInteraction card)
    {
        Hand.Remove(card);
    }

    // Iterates through selected cards to access them during resolve step.
    public IEnumerable<CardInteraction> GetSelectedCards() // Interface Enumerable - Defines an Interface that uses an Enumerator to traverse an Element Sequence (In this case, of 'CardInteraction' objects!).
    {
        foreach (var c in Hand)
        {
            if (c.IsSelected) yield return c; // 'yield' - Pauses execution after finding first selected card, continues once called again from where it left off.
        }
    }

    // Creates UI Card Prefab in the hand for each Elemental SO in the deck.
    public CardInteraction SpawnCard(PlayableCardDef def, ElementalCombat owner = null)
    {
        var go = Instantiate(cardPrefab, transform); // Parent GameObject under the Hand Manager in the Canvas.
        var display = go.GetComponent<PlayableCardDisplay>();
        var card = go.GetComponent<CardInteraction>();

        display.playableCard = def;
        card.ownerElemental = owner ? owner : defaultOwnerElemental;

        AddToHand(card);
        return card;
    }

    // Discards played card from the Player's hand and then destroys them.
    public void Discard(CardInteraction card, PlayerDeck playerDeck)
    {
        if (card != null)
        {
            var def = card.GetComponent<PlayableCardDisplay>()?.playableCard;
            if (playerDeck && def) playerDeck.Discard(def);

            RemoveFromHand(card);
            Destroy(card.gameObject);
        }
    }
}
