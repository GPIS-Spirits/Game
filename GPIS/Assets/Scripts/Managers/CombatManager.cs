// Manager to resolve combat actions between player & enemy Elementals.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CombatManager : MonoBehaviour
{
    public static CombatManager Instance { get; private set; }

    [Header("Game Object Wiring")]
    public HandManager handManager;

    // List of active enemies in the current battle:
    public List<EnemyCombat> activeEnemies = new();
    public EnemyCombat selectedEnemy;

    public PlayerCombat playerCombat;
    public PlayerDeck playerDeck;

    [Header("Enemy Spawning")]
    public Transform[] enemySpawnPoints;
    public GameObject enemyPrefab;

    public static int enemyCount;

    public void Start()
    {
        SpawnEnemies();
    }

    void Awake()
    {
        if (Instance != null && Instance != this) Destroy(this);
        else Instance = this;
    }

    void Update()
    {
        // Handle mouse clicks for enemy selection:
        if (Input.GetMouseButtonDown(0))
        {
            var cam = Camera.main;
            if (cam == null) return;

            // Create a 2D Raycast to collide with Enemy:
            Vector3 worldPoint = cam.ScreenToWorldPoint(Input.mousePosition);
            Vector2 wp2 = new Vector2(worldPoint.x, worldPoint.y);
            var hit2D = Physics2D.Raycast(wp2, Vector2.zero);
            if (hit2D.collider != null)
            {
                var enemy = hit2D.collider.GetComponentInParent<EnemyCombat>();
                if (enemy != null)
                {
                    Debug.Log($"CombatManager: clicked 2D enemy {enemy.name}");
                    SelectEnemy(enemy);
                    return;
                }
            }
        }
    }

    // Calculates damage based on stats of passed 'ElementalCombat.Def'.
    private int ComputeAttackDamage(ElementalCombat elemental)
    {
        float raw = elemental.Def.baseDmg; // Damage = baseDmg
        return Mathf.Max(0, Mathf.RoundToInt(raw));
    }

    // Spawns enemies at fixed spawn points (variable chance to spawn at each spot).
    public void SpawnEnemies()
    {
        activeEnemies.Clear();

        if (enemySpawnPoints == null || enemySpawnPoints.Length == 0 || enemyPrefab == null)
            return;

        foreach (var sp in enemySpawnPoints)
        {
            if (sp == null) continue;
            if (Random.value <= 0.6f)
            {
                var go = Instantiate(enemyPrefab, sp.position, sp.rotation);
                var ec = go.GetComponent<EnemyCombat>();
                if (ec != null)
                {
                    activeEnemies.Add(ec);
                }
            }
        }

        // Update "enemyCount" with List:
        enemyCount = activeEnemies.Count;
    }

    // Select or toggle an enemy as the current target.
    public void SelectEnemy(EnemyCombat enemy)
    {
        Debug.Log($"CombatManager.SelectEnemy called for: { (enemy != null ? enemy.name : "null") }");

        if (selectedEnemy == enemy)
        {
            if (selectedEnemy != null) selectedEnemy.SetHighlighted(false);
            selectedEnemy = null;
            return;
        }

        if (selectedEnemy != null) selectedEnemy.SetHighlighted(false);
        selectedEnemy = enemy;
        if (selectedEnemy != null) selectedEnemy.SetHighlighted(true);
    }

    // Allows CombatManager to remove dead enemy from lists for clean up.
    public void OnEnemyDeath(EnemyCombat enemy)
    {
        if (enemy == null) return;

        // Remove from active list if present:
        if (activeEnemies.Contains(enemy)) activeEnemies.Remove(enemy);

        // Update global count and clear selection if necessary:
        enemyCount = activeEnemies.Count;
        if (selectedEnemy == enemy) selectedEnemy = null;

        // Destroy enemy GameObject:
        if (enemy.gameObject != null)
            Destroy(enemy.gameObject);

        // Check for end of battle:
        isBattleOver();
    }

    // Implements the Resolve Step when the "Play Cards" button is pressed.
    public void OnPlayCardsPressed()
    {

        if (enemyCount <= 0) return; // Require at least one enemy alive:

        var toDiscard = new System.Collections.Generic.List<CardInteraction>();

        foreach (var card in handManager.GetSelectedCards())
        {
            if (card.ownerElemental == null) continue;

            if (card.Display.playableCard.actionType == CardActionType.Attack)
            {
                // Choose target ("selectedEnemy", otherwise first active enemy):
                EnemyCombat target = selectedEnemy;
                if (target == null && activeEnemies.Count > 0) target = activeEnemies[0];
                if (target == null) continue; // No target available:

                int dmg = ComputeAttackDamage(card.ownerElemental);
                target.TakeDamage(dmg);
            }
            else if (card.Display.playableCard.actionType == CardActionType.Defend)
            {
                playerCombat.Heal(1);
            }

            toDiscard.Add(card);
        }

        foreach (var c in toDiscard)
        {
            handManager.Discard(c, playerDeck);
        }
    }

    public static void isBattleOver()
    {
        // Try using Instance "activeEnemies" count if available:
        int remaining = (Instance != null) ? Instance.activeEnemies.Count : enemyCount;

        if (remaining <= 0)
        {
            Debug.Log("All enemies defeated! Battle Over.");

            // Find the hand manager in scene and clear it:
            var hm = Object.FindObjectOfType<HandManager>();
            if (hm != null)
                hm.ClearHand();

            GameManager.Instance.ExitBattle();
        }
    }
}
