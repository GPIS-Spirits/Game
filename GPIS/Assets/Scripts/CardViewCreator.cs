using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System; // Needed for DOScale

public class CardViewCreator : Singleton<CardViewCreator>
{
    [SerializeField] private CardView cardPrefab;

    // Creates a CardView at the specified position and rotation with a scaling animation
    public CardView CreateCardView(Vector2 position, Quaternion rotation)
    {
        CardView cardView = Instantiate(cardPrefab, position, rotation);
        cardView.transform.localScale = Vector2.zero; // Set scale to zero
        cardView.transform.DOScale(Vector2.one, 0.15f); // Animate scale to one
        return cardView;
    }
}
