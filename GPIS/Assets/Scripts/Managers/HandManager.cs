using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


public class HandManager : MonoBehaviour
{
    [Header("Card UI Prefab")]
    [SerializeField] private GameObject cardPrefab; // assign PlayableCard_UI prefab here
    [SerializeField] private ElementalCombat defaultOwnerElemental;

    public const int maxHandsize = 5;
    //public static int handSize = 0;
    public int HandSize => Hand.Count;

    [Header("Curve Settings (parabola)")]
    [SerializeField] private RectTransform Vertex;
    [Tooltip("Horizontal spacing between card centers")]
    public float padding = 120f;
    [Tooltip("Parabola strength (your old 'e' value)")]
    public float curveStrength = 0.025f;
    [Tooltip("Global offset from vertex in direction of the HandManager transform's up")]
    public float offset = 100f;

    [Header("Animation Settings")]
    public float moveSpeed = 15f;   // bigger = faster move smoothing
    public float rotateSpeed = 12f; // bigger = faster rotation smoothing

    // The hand list
    public readonly List<CardInteraction> Hand = new();

    // Spawns a card prefab, configures it, parents it to this HandManager, and returns the CardInteraction.
    // Matches the earlier usage in TurnManager.DrawStep(...)
    public CardInteraction SpawnCard(PlayableCardDef def, ElementalCombat owner = null)
    {
        if (!cardPrefab)
        {
            Debug.LogError("HandManager.SpawnCard: cardPrefab is not assigned.");
            return null;
        }

        var go = Instantiate(cardPrefab, this.transform); // parent under HandManager
        var display = go.GetComponent<PlayableCardDisplay>();
        var card = go.GetComponent<CardInteraction>();

        if (!card)
        {
            Debug.LogError("HandManager.SpawnCard: card prefab missing CardInteraction component.");
            Destroy(go);
            return null;
        }

        if (display != null)
            display.playableCard = def; // keep your original usage

        card.ownerElemental = owner ? owner : defaultOwnerElemental;

        AddToHand(card);

        return card;
    }

    // Returns all currently selected cards (keeps compatibility)
    public IEnumerable<CardInteraction> GetSelectedCards()
    {
        foreach (var c in Hand)
            if (c.IsSelected) yield return c;
    }

    // Discard card and (optionally) notify the player's deck
    public void Discard(CardInteraction card, PlayerDeck playerDeck)
    {
        if (card == null) return;

        // grab definition the way your existing code expects:
        var def = card.GetComponent<PlayableCardDisplay>()?.playableCard;
        if (playerDeck && def) playerDeck.Discard(def);

        RemoveFromHand(card);
        Destroy(card.gameObject);
    }

    // -------------------
    // Add / Remove helpers
    // -------------------

    public void AddToHand(CardInteraction card)
    {
        if (card == null) return;
        if (!Hand.Contains(card))
        {
            Hand.Add(card);
            //handSize += 1;

            // Ensures correct visual layer ordering (left = bottom, right = top)
            card.transform.SetSiblingIndex(Hand.Count - 1);

            // Debug for the cards being added with the right index
            Debug.Log($"[HandManager] Added card '{card.name}' at sibling index {card.transform.GetSiblingIndex()} (Hand count: {Hand.Count})");

            // immediately compute targets so it animates in
            UpdateCardTargets();
        }
    }

    public void RemoveFromHand(CardInteraction card)
    {
        if (card == null) return;
        if (Hand.Remove(card))
        {
            // Ensure there is no gaps and is left-to-right bottom-to-top for stacking
            for (int i = 0; i < Hand.Count; i++)
            {
                Hand[i].transform.SetSiblingIndex(i);
            }

            // Show the order after removal
            //Debug.Log("[HandManager] Current sibling order after removal:");
            //for (int i = 0; i < Hand.Count; i++)
            //{
            //    Debug.Log($"   Card {Hand[i].name} -> sibling {Hand[i].transform.GetSiblingIndex()}");
            //}

            // Recalculate positions on the curve
            UpdateCardTargets();

            //HandManager.handSize -= 1;
        }
    }

    // Clears all cards from the hand and destroys their GameObjects.
    public void ClearHand()
    {
        // iterate over a copy to avoid modifying the collection while enumerating
        foreach (var card in Hand.ToArray())
        {
            if (card == null) continue;
            RemoveFromHand(card);
            if (card.gameObject != null)
                Destroy(card.gameObject);
        }

        Hand.Clear();
        UpdateCardTargets();
    }

    // -------------------
    // Layout & animation
    // -------------------

    // Thought it best to call it every frame to make sure it was smooth.
    void Update()
    {
        if (Hand.Count > 0)
            UpdateCardTargets();
    }

    // Calculates the parabolic positions/rotations and assigns them to each CardInteraction's target fields.
    private void UpdateCardTargets()
    {
        if (Vertex == null || Hand.Count == 0) return;

        int numberOfCards = Hand.Count;
        float halfCount = (numberOfCards - 1) / 2f;

        for (int i = 0; i < numberOfCards; i++)
        {
            float x = padding * (i - halfCount);
            float y = -Mathf.Pow(curveStrength * x, 2); // parabola: - (e * x)^2.... math

            // World position for UI RectTransform (use Vertex.position so it follows the inspector object)
            Vector3 targetPosition = new Vector3(x, y, 0f) + Vertex.position + (transform.up * offset);

            // compute tangent-based rotation (Z)
            float delta = 0.01f;
            float y1 = -Mathf.Pow(curveStrength * (x - delta), 2);
            float y2 = -Mathf.Pow(curveStrength * (x + delta), 2);
            float dy_dx = (y2 - y1) / (2f * delta);
            float angleZ = Mathf.Atan2(dy_dx, 1f) * Mathf.Rad2Deg;
            Quaternion targetRotation = Quaternion.Euler(0f, 0f, angleZ);

            // Assign targets to the card's fields
            var card = Hand[i];

            //var hasTargetFields = true; // I will assume all cards have these fields for now, but you can add a check later if needed
            
            card.targetPos = targetPosition;
            card.targetRot = targetRotation;

            // If the card doesn't have the new fields, we skip assigning them.
            // This allows older CardInteraction scripts to still work without modification.
            card.moveSpeed = moveSpeed;
            card.rotateSpeed = rotateSpeed;
        }
    }
}
