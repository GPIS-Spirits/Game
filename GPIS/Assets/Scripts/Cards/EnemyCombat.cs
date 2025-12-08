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

    private bool isHighlighted = false;

    // Renderers for Spirit Highlights:
    private SpriteRenderer[] spriteRenderers;
    private Color[] spriteOriginalColors;
    private Renderer[] otherRenderers;
    private Color[] otherOriginalColors;

    [SerializeField] private Color highlightColor = Color.red;

    void Awake()
    {
        if (!enemyDisplay) enemyDisplay = GetComponent<EnemyDisplay>();
        CurrentHP = MaxHP; // Set HP at Awake():

        CombatManager.enemyCount += 1;

        // Cache sprite renderers and their original colors:
        spriteRenderers = GetComponentsInChildren<SpriteRenderer>(true);
        spriteOriginalColors = new Color[spriteRenderers.Length];
        for (int i = 0; i < spriteRenderers.Length; i++)
            spriteOriginalColors[i] = spriteRenderers[i].color;

        // Cache other renderers (exclude sprite renderers):
        var allRenderers = GetComponentsInChildren<Renderer>(true);
        var list = new List<Renderer>();
        foreach (var r in allRenderers)
        {
            if (r is SpriteRenderer) continue;
            list.Add(r);
        }
        otherRenderers = list.ToArray();
        otherOriginalColors = new Color[otherRenderers.Length];
        for (int i = 0; i < otherRenderers.Length; i++)
        {
            // Store material color if available:
            otherOriginalColors[i] = otherRenderers[i].material.color;
        }
    }

    // Takes in final damage number (calculated outside of 'EnemyCombat' Class) to apply to this enemy.
    public void TakeDamage(int amount)
    {
        CurrentHP -= amount;
        Debug.Log($"Enemy took {amount} damage. HP: {CurrentHP}/{MaxHP}");

        if (CurrentHP <= 0) {
            // Notify CombatManager to destroy Enemy:
            var manager = Object.FindObjectOfType<CombatManager>();
            if (manager != null) manager.OnEnemyDeath(this);
            else {
                Destroy(gameObject);
                CombatManager.enemyCount -= 1;
                CombatManager.isBattleOver();
            }
        }
    }

    public void SetHighlighted(bool on)
    {
        isHighlighted = on;

        // Update sprite renderers:
        if (spriteRenderers != null)
        {
            for (int i = 0; i < spriteRenderers.Length; i++)
            {
                if (spriteRenderers[i] == null) continue;
                spriteRenderers[i].color = on ? highlightColor : spriteOriginalColors[i];
            }
        }

        // Update other renderers:
        if (otherRenderers != null)
        {
            for (int i = 0; i < otherRenderers.Length; i++)
            {
                if (otherRenderers[i] == null) continue;
                otherRenderers[i].material.color = on ? highlightColor : otherOriginalColors[i];
            }
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

    // Selects enemy when clicked to target for attack cards.
    void OnMouseDown()
    {
        Debug.Log($"EnemyCombat.OnMouseDown on {gameObject.name}");

        var manager = Object.FindObjectOfType<CombatManager>();
        if (manager != null) manager.SelectEnemy(this);
    }
}