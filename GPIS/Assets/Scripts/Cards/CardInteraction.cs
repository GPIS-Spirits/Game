// Attached to playable cards to define their actions & functionality based on "PlayableCardDef".

using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(PlayableCardDisplay))]
public class CardInteraction : MonoBehaviour
{
    [Header("Game Object Wiring")]
    public ElementalCombat ownerElemental;
    public Image selectionHighlight;

    public bool IsSelected { get; private set; }
    public PlayableCardDisplay Display { get; private set; }

    // ------------------------------------------------
    // Smooth animation fields (new)
    // ------------------------------------------------
    [HideInInspector] public Vector3 targetPos;
    [HideInInspector] public Quaternion targetRot = Quaternion.identity;

    [Header("Animation Settings - per card (overwritten by HandManager)")]
    public float moveSpeed = 15f;
    public float rotateSpeed = 12f;
    // ------------------------------------------------

    void Awake()
    {
        Display = GetComponent<PlayableCardDisplay>();
        SetSelected(false);

        // initialize targets to current transform so we don't jump at spawn
        targetPos = transform.position;
        targetRot = transform.rotation;
    }

    void Start()
    {
        var handManager = GetComponentInParent<HandManager>();
        if (handManager)
            handManager.AddToHand(this);
        else
            Debug.LogWarning("CardInteraction: No HandManager found in parents.");
    }

    public void OnCardClicked()
    {
        SetSelected(!IsSelected);
    }

    public void SetSelected(bool selected)
    {
        // Allows only 2 cards to be selected at a time before pressing "Play Cards":
        if (selected && !IsSelected)
        {
            var handManager = GetComponentInParent<HandManager>();
            if (handManager != null)
            {
                int selectedCount = 0;
                foreach (var c in handManager.GetSelectedCards())
                {
                    selectedCount++;
                    if (selectedCount >= 2) break;
                }

                if (selectedCount >= 2)
                {
                    Debug.Log("Maximum of 2 cards may be selected at once.");
                    return;
                }
            }
        }

        IsSelected = selected;
        if (selectionHighlight) selectionHighlight.enabled = selected;
    }

    void OnDestroy()
    {
        var handManager = GetComponentInParent<HandManager>();
        if (handManager) handManager.RemoveFromHand(this);
    }

    // Smoothly move / rotate toward targets every frame
    void Update()
    {
        // If no meaningful target was assigned, this keeps the card where it is.
        transform.position = Vector3.Lerp(transform.position, targetPos, Time.deltaTime * moveSpeed);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, Time.deltaTime * rotateSpeed);
    }
}
