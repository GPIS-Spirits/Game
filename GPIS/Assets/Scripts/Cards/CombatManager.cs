// Manager to resolve combat actions between player & enemy Elementals.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatManager : MonoBehaviour
{
    [Header("Game Object Wiring")]
    public HandManager handManager;
    public EnemyCombat activeEnemy;

    private int ComputeAttackDamage(ElementalCombat elemental)
    {
        float raw = elemental.Def.dmgFlat * elemental.Def.dmgMult;
        return Mathf.Max(0, Mathf.RoundToInt(raw)); // Always returns the raw damage as an int rounded up:
    }

    public void OnPlayCardsPressed()
    {
        if (!activeEnemy) return; // There is no enemy to fight:

        var toDiscard = new System.Collections.Generic.List<CardInteraction>(); // Creates a new 'List' container to hold played cards so they can be discarded.

        foreach (var card in handManager.GetSelectedCards())
        {
            if (card.ownerElemental == null) continue;

            int dmg = ComputeAttackDamage(card.ownerElemental);
            activeEnemy.TakeDamage(dmg);
            toDiscard.Add(card); // Adds played cards to the List 'toDiscard' to "mark" them for removal.
        }

        foreach (var c in toDiscard) // For each played card that has been added to 'toDiscard':
        {
            handManager.Discard(c);
        }
    }
}
