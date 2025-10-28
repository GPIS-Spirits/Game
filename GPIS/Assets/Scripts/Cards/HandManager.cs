// Manager for accessing/manipulating cards in hand during combat.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandManager : MonoBehaviour
{
    public readonly List<CardInteraction> Hand = new(); // List to hold all cards in the Player's hand.

    public void AddToHand(CardInteraction card)
    {
        if (!Hand.Contains(card)) Hand.Add(card);
    }

    public void RemoveFromHand(CardInteraction card)
    {
        Hand.Remove(card);
    }

    // Iterates through selected cards to access them during resolve step.
    public IEnumerable<CardInteraction> GetSelectedCards()
    {
        foreach (var c in Hand)
        {
            if (c.IsSelected) yield return c;
        }
    }

    // Discards played cards from the Player's hand and then destroys them.
    public void Discard(CardInteraction card)
    {
        RemoveFromHand(card);
        Destroy(card.gameObject);
    }
}
