// Manager for Player & Enemy turns during a battle.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnManager : MonoBehaviour
{
    public enum Phase { Player, Enemy }
    public Phase CurrentPhase { get; private set; } = Phase.Player;

    [Header("Game Object Wiring")]
    public CombatManager combatManager;
    public PlayerCombat player;
    public PlayerDeck playerDeck;
    public HandManager handManager;

    [Header("Turn Settings")]
    public int startingHandSize;
    public int drawPerPlayerTurn = 1;

    void Start()
    {
        DrawStep(startingHandSize);
    }

    public void OnEndTurnPressed()
    {
        if (CurrentPhase != Phase.Player) return; // Don't let the player end the turn if it isn't their turn:

        StartEnemyTurn();
        Debug.Log("Enemy Turn Started.");
    }

    private void StartEnemyTurn()
    {
        CurrentPhase = Phase.Enemy;

        // --- Enemy AI Decision Tree implementation would go here, but simple attack for now! ---
        if (combatManager != null && combatManager.activeEnemies != null && combatManager.activeEnemies.Count > 0)
        {
            foreach (var enemy in combatManager.activeEnemies)
            {
                if (enemy != null)
                    enemy.PerformAttack(player);
            }
        }

        StartPlayerTurn(); // Switch the turn back to the Player after enemy attacks:
    }

    private void StartPlayerTurn()
    {
        CurrentPhase = Phase.Player;
        DrawStep(drawPerPlayerTurn);
        Debug.Log("Player Turn Started.");
    }

    private void DrawStep(int n)
    {
        // Use the HandManager instance's HandSize instead of the old static counter
        if (!playerDeck || !handManager || n <= 0 || handManager.HandSize >= HandManager.maxHandsize) return;

        var buffer = new List<PlayableCardDef>();
        int drawn = playerDeck.TryDraw(n, buffer);

        for (int i = 0; i < drawn; i++)
        {
            handManager.SpawnCard(buffer[i]);
        }
    }
}
