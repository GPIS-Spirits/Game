// Accesses the "EnemyDef" of an Enemy to use for combat mechanics.

using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyCombat : MonoBehaviour
{
    [SerializeField] private EnemyDisplay enemyDisplay;

    public int CurrentHP { get; private set; }
    public int MaxHP => enemyDisplay.enemy.maxHealth;

    void Awake()
    {
        if (!enemyDisplay) enemyDisplay = GetComponent<EnemyDisplay>();
        CurrentHP = MaxHP;
    }

    public void TakeDamage(int amount)
    {
        CurrentHP -= amount;
        Debug.Log($"Enemy took {amount} damage. HP: {CurrentHP}/{MaxHP}");
    }
}
