using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEditorInternal;
using UnityEngine;

[RequireComponent(typeof(PlayerDisplay))]
public class PlayerCombat : MonoBehaviour
{
    [SerializeField] private PlayerDisplay playerDisplay;

    public int CurrentHP { get; private set; } // Auto-Implemented Get/Set
    public int MaxHP => playerDisplay.player.MaxHP;

    void Awake()
    {
        if (!playerDisplay) playerDisplay = GetComponent<PlayerDisplay>();

        CurrentHP = MaxHP;
        Debug.Log($"Player HP: {CurrentHP}/{MaxHP}");
    }

    public void TakeDamage (int amount)
    {
        amount = Mathf.Max(0, amount);
        CurrentHP = Mathf.Max(0, CurrentHP - amount);
        Debug.Log($"Player took {amount} damage. HP: {CurrentHP}/{MaxHP}");

        if (CurrentHP <= 0)
        {
            Debug.Log("The Player has been defeated!");

            // --- Add Player Lose logic here! ---
        }
    }
}
