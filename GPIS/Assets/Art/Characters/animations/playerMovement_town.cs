using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(SpriteRenderer))]
public class PlayerMovement : MonoBehaviour
{
    //public TextMeshProUGUI collectibleText;
    //public TextMeshProUGUI timerText;

    Animator anim;
    SpriteRenderer spriteRenderer;
    Rigidbody2D rb2d;

    public float speed;

    // int collectibleCount = 0;
    // float timer = 0;

    //house
    bool nearDoor = false; //is player near door
    Door currentDoor = null; //referece to the door that player is near
    public TextMeshProUGUI interactText; // Assign in Inspector

    public Sprite left, right, up, downleft, downright;

    private bool lookingLeft = false;

    // Start is called before the first frame update
    void Start()
    {
        rb2d = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        speed = 5f;

        // Subscribe to scene load event to move player to spawn point
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        string spawnPointName = PlayerPrefs.GetString("SpawnPointName", "");
        if (!string.IsNullOrEmpty(spawnPointName))
        {
            GameObject spawnPoint = GameObject.Find(spawnPointName);
            if (spawnPoint != null)
            {
                transform.position = spawnPoint.transform.position;
            }
            else
            {
                Debug.LogWarning($"Spawn point '{spawnPointName}' not found in the scene.");
            }
            // Clear the spawn point name after use
            PlayerPrefs.DeleteKey("SpawnPointName");
        }
    }

    void Update()
    {
        // timer += Time.deltaTime % 60;
        // timerText.text = timer.ToString("0");

        Move();
        Interact();
    }

    void Interact()
    {
        if (nearDoor && currentDoor != null && Input.GetKeyDown(KeyCode. E))
        {
            EnterDoor();
        }
    }

    void EnterDoor()
    {
        // Store the spawn point name so we can access it after scene load
        PlayerPrefs.SetString("SpawnPointName", currentDoor.spawnPointName);
        SceneManager.LoadScene(currentDoor.sceneToLoad);
    }

    void Move()
    {
        Vector2 move;

        if (Input.GetKey(KeyCode. A)) move.x = -speed;
        else if (Input.GetKey(KeyCode. D)) move.x = speed;
        else move.x = 0;

        if (Input.GetKey(KeyCode. W)) move.y = speed;
        else if (Input.GetKey(KeyCode. S)) move.y = -speed;
        else move.y = 0;

        if (move.x < 0)
        {
            spriteRenderer.sprite = left;
            lookingLeft = true;
        } else if (move.x > 0)
            {
                spriteRenderer.sprite = right;
                lookingLeft = false;
            }

        if (move.y > 0) // moving UP
        {
            spriteRenderer.sprite = up;
        }else if (move.y < 0)
        {
            spriteRenderer.sprite = (lookingLeft) ? downleft : downright;
        }

        rb2d.velocity = move;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        //if (other.CompareTag("Collectible"))
        //{
        //    other.gameObject.SetActive(false);
        //    collectibleCount++;
        //    collectibleText.text = ": " + collectibleCount;

        //    if (collectibleCount >= 8)
        //    {
        //        SceneManager.LoadScene("CreditScreen");
        //    }
        //}

        if (other.CompareTag("Door"))
        {

            currentDoor = other.GetComponent<Door>();
            if (currentDoor != null)
            {
                nearDoor = true;
                if (interactText != null)
                {
                    interactText.text = $"Press E to Enter";
                    interactText.gameObject.SetActive(true);
                }
            }
        }
    }
    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Door"))
        {
            nearDoor = false;
            currentDoor = null;
            if (interactText != null)
                interactText.gameObject.SetActive(false);
        }
    }

    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}