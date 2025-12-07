// Manager to resolve combat actions between player & enemy Elementals.

/*
Need to Add:
    - Player Game Object (to take damage from Enemies) X
    - Turn Manager X
        - Enemy Turn X
    - Enemy Attacking X
    - Deck System X
        - Draw Step X
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CombatManager : MonoBehaviour
{
    [Header("Game Object Wiring")]
    public HandManager handManager;
    public EnemyCombat activeEnemy;
    public PlayerCombat playerCombat;
    public PlayerDeck playerDeck;

    public static int enemyCount;

    // Calculates damage based on stats of passed 'ElementalCombat.Def'.
    private int ComputeAttackDamage(ElementalCombat elemental)
    {
        float raw = elemental.Def.baseDmg; // Damage = baseDmg
        return Mathf.Max(0, Mathf.RoundToInt(raw)); // 'Mathf.Max()' - Returns higher number of inputs. | 'RoundToInt()' - Rounds non-ints to nearest int.
    }

    // Implements the Resolve Step when the "Play Cards" button is pressed.
    public void OnPlayCardsPressed()
    {
        if (!activeEnemy) return; // There is no enemy to fight:

        var toDiscard = new System.Collections.Generic.List<CardInteraction>(); // Creates a new Generic 'List' container to hold played cards so they can be discarded.

        foreach (var card in handManager.GetSelectedCards())
        {
            if (card.ownerElemental == null) continue; // No current Elemental on the field connected to this card: 

            // Checks card type and evokes the appropriate action method:
            if (card.Display.playableCard.actionType == CardActionType.Attack) // If the played card is an Attack type:
            {
                // Compute damage and apply to active enemy:
                int dmg = ComputeAttackDamage(card.ownerElemental);
                activeEnemy.TakeDamage(dmg);
            }
            else if (card.Display.playableCard.actionType == CardActionType.Defend) // If the played card is a Defend (Heal) type:
            {
                playerCombat.Heal(1);
            }

            toDiscard.Add(card); // Adds played cards to the List 'toDiscard' to "mark" them for removal.
        }

        foreach (var c in toDiscard) // For each played card that has been added to 'toDiscard':
        {
            handManager.Discard(c, playerDeck);
        }
    }

    public static void isBattleOver()
    {
        if (enemyCount <= 0)
        {
            Debug.Log("All enemies defeated! Battle Over.");

            // Find the hand manager in scene and clear it:
            var hm = Object.FindObjectOfType<HandManager>();
            if (hm != null)
                hm.ClearHand();

            GameManager.Instance.ExitBattle(); // Equivalent to "SceneManager.UnloadSceneAsync("battle");" using the GameManager:
        }
    }
}
