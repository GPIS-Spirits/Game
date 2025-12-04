// Accesses the "EnemyDef" of an Enemy to use for combat mechanics.

using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyCombat : MonoBehaviour
{
    [SerializeField] private EnemyDisplay enemyDisplay;

    public int CurrentHP { get; private set; } // "Auto-Implemented Get/Set" - C# shortcut that allows "CurrentHP" to be accessed publicly, set privately.
    public int MaxHP => enemyDisplay.enemy.maxHealth; // Lambda Operator '=>' - Shorthand Getter definition for 'MaxHP'.

    void Awake()
    {
        if (!enemyDisplay) enemyDisplay = GetComponent<EnemyDisplay>();
        CurrentHP = MaxHP; // Set HP at Awake():

        CombatManager.enemyCount += 1;
    }

    // Takes in final damage number (calculated outside of 'EnemyCombat' Class) to apply to this enemy.
    public void TakeDamage(int amount)
    {
        CurrentHP -= amount;
        Debug.Log($"Enemy took {amount} damage. HP: {CurrentHP}/{MaxHP}");

        if (CurrentHP <= 0) {
            Destroy(gameObject);

            CombatManager.enemyCount -= 1;
            CombatManager.isBattleOver(); // Checks to see if 'enemyCount' is 0 to end the battle and reload 'dunegonLoop':
        }
    }

    public int GetAttackValue()
    {
        return enemyDisplay.enemy.attack; // Grabs 'attack' stat from 'EnemyDef':
    }

    public void PerformAttack(PlayerCombat player)
    {
        if (!player) return; // No 'player' was selected for the attack:

        int dmg = Mathf.Max(0, GetAttackValue());
        player.TakeDamage(dmg);
        Debug.Log($"Enemy attacked Player for {dmg} damage!");
    }
}
